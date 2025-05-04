using System.Data;
using System.Diagnostics;
using System.Text;
using ClosedXML.Excel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.DTOMapping;
using UserManagment.Application.SystemConfiguration;
using UserManagment.Common.DTO.UserDTo;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.Users
{
    public record UserUploadRequestDTO(IFormFile File) : IRequest<object>;

    public class UserUploadHandler : IRequestHandler<UserUploadRequestDTO, object>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public UserUploadHandler(IUnitOfWork unitOfWork, IServiceProvider serviceProvider, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _serviceProvider = serviceProvider;
            _mediator = mediator;
        }


        public async Task<object> Handle(UserUploadRequestDTO request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
                throw new CustomException("File is empty or null");

            var stopwatch = new Stopwatch();
            stopwatch.Start();  // Start the stopwatch

            var userResults = new List<UserResultDto>();
            var userRequests = new List<UserRequestDTO>();

            var emailDictionary = new Dictionary<string, bool>(); // Email and validation status
            var groupDictionary = new Dictionary<string, Guid>(); // Group code and corresponding ID

            var validationLog = new StringBuilder();
            var excludedUsers = new List<ExcludedUsersDto>();

            var domains = await _mediator.Send(new DomainFormat(), cancellationToken);
            var validDomains = domains.Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(d => d.Trim()).ToList(); //all valid domains

            var groupCodesSet = new HashSet<string>(); // Collect all unique group codes
                                                       // Dictionary to hold email to group codes mapping
            var emailToGroupCodes = new Dictionary<string, string>();
            // Single pass for file processing
            using (var stream = new MemoryStream())
            {
                await request.File.CopyToAsync(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rowCount = worksheet.LastRowUsed().RowNumber();

                    string email, firstName, lastName, mobileNumber, location, groupCodesString;
                    DateTime parsedDate;
                    List<string> groupCodes;
                    var tryParseDate = new DateOnly();

                    // First pass: Collect emails and group codes, initialize email dictionary
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var rowCells = worksheet.Row(row).Cells();  // Avoid calling Cell() multiple times

                        email = rowCells.ElementAt(2).GetString(); // Assuming column 3 is email
                        groupCodesString = rowCells.Count() > 6 ? rowCells.ElementAt(6).GetString() : string.Empty;
                        groupCodes = groupCodesString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                     .Select(g => g.Trim())
                                                     .ToList();
                        DateTime.TryParse(rowCells.ElementAt(5).GetString(), out parsedDate);
                        tryParseDate = DateOnly.FromDateTime(parsedDate);

                        // Initialize email status and collect group codes
                        if (!emailDictionary.ContainsKey(email))
                            emailDictionary[email] = false;  // Mark as invalid until validation

                        emailToGroupCodes[email] = groupCodesString; // Save for later retrieval

                        foreach (var code in groupCodes)
                        {
                            groupCodesSet.Add(code); // Collect unique group codes
                        }
                    }

                    // Fetch all group IDs and codes in one query (batch database request)
                    var groupIds = await _unitOfWork.Group.GetGroupIdsByCodesAsync(groupCodesSet.ToList());
                    foreach (var (code, id) in groupIds)
                    {
                        groupDictionary[code] = id;
                    }

                    // Validate emails in bulk (batch request for checking duplicates)
                    var emailsToCheck = emailDictionary.Keys.ToList();
                    var emailCheckResults = await _unitOfWork.Authentication.CheckEmailsInBulk(emailsToCheck);

                    foreach (var mail in emailCheckResults)
                    {
                        // Validate domain
                        if (!validDomains.Contains(mail.Key.Split('@').Last()))
                        {
                            validationLog.AppendLine($"User with email {mail.Key} has an invalid domain and will be excluded.");
                            emailDictionary[mail.Key] = false; // Mark as invalid

                            // Retrieve the groupCodesString from the dictionary
                            string groupCodesForExcludedUser = emailToGroupCodes.ContainsKey(mail.Key)
                                ? emailToGroupCodes[mail.Key]
                                : string.Empty;
                            excludedUsers.Add(new ExcludedUsersDto
                            {
                                Email = mail.Key,
                                DateOfBirth = tryParseDate,
                                Groups = groupCodesForExcludedUser,
                                Exception = ErrorResponseMessage.User_MailDomain
                            });
                            continue;
                        }

                        // Validate duplicates based on the CheckEmailsInBulk results
                        if (mail.Value)
                        {
                            validationLog.AppendLine($"User with email {mail.Key} already exists and will be excluded.");
                            emailDictionary[mail.Key] = false; // Mark as invalid
                            // Retrieve the groupCodesString from the dictionary
                            string groupCodesForExcludedUser = emailToGroupCodes.ContainsKey(mail.Key)
                                ? emailToGroupCodes[mail.Key]
                                : string.Empty;
                            excludedUsers.Add(new ExcludedUsersDto
                            {
                                Email = mail.Key,
                                DateOfBirth = DateOnly.FromDateTime(DateTime.Now),
                                Groups = groupCodesForExcludedUser,
                                Exception = "Email already exists"
                            });
                            continue;
                        }

                        // Mark valid emails
                        validationLog.AppendLine($"User with email {mail.Key} is valid and will be processed.");
                        emailDictionary[mail.Key] = true;
                    }

                    // Second pass: Process valid users
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var rowCells = worksheet.Row(row).Cells();
                        email = rowCells.ElementAt(2).GetString();

                        // Skip invalid emails
                        if (!emailDictionary[email]) continue;

                        // Assign values once to minimize method calls
                        firstName = rowCells.ElementAt(0).GetString();  // First name
                        lastName = rowCells.ElementAt(1).GetString();   // Last name
                        mobileNumber = rowCells.ElementAt(3).GetString();  // Mobile number
                        location = rowCells.ElementAt(4).GetString();   // Location

                        DateTime.TryParse(rowCells.ElementAt(5).ToString(), out parsedDate); // Date

                        groupCodesString = rowCells.Count() > 6 ? rowCells.ElementAt(6).GetString() : string.Empty;// Group codes
                        groupCodes = groupCodesString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                     .Select(g => g.Trim())
                                                     .ToList(); // separate group codes


                        //check if the user have atleast one group
                        if (!groupCodes.Any())
                        {
                            excludedUsers.Add(new ExcludedUsersDto
                            {
                                Email = email,
                                DateOfBirth = tryParseDate,
                                Groups = groupCodesString,
                                Exception = ErrorResponseMessage.NoGroups
                            });
                            continue; // Skip processing this user
                        }
                        // Validate group codes
                        var invalidGroupCodes = groupCodes.Where(code => !groupDictionary.ContainsKey(code)).ToList();
                        if (invalidGroupCodes.Any())
                        {
                            excludedUsers.Add(new ExcludedUsersDto
                            {
                                Email = email,
                                DateOfBirth = tryParseDate,
                                Groups = groupCodesString,
                                Exception = ErrorResponseMessage.Group_NotFound
                            });
                            continue; // Skip processing this user
                        }

                        // Fetch corresponding group IDs
                        var groupIdsForUser = groupCodes.Select(code => groupDictionary[code]).ToList();

                        userRequests.Add(new UserRequestDTO(firstName, lastName, email, mobileNumber, location, parsedDate, groupIdsForUser));
                    }
                }
            }

            validationLog.AppendLine($"{userRequests.Count} users were successfully added out of {emailDictionary.Count}.");
            Console.WriteLine(validationLog.ToString());

            // Process users in parallel batches
            const int batchSize = 10;
            var tasks = new List<Task<UserResultDto>>();
            for (int i = 0; i < userRequests.Count; i += batchSize)
            {
                var batch = userRequests.Skip(i).Take(batchSize).ToList();
                tasks.AddRange(batch.Select(userRequest =>
                    Task.Run(async () =>
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                            return await mediator.Send(userRequest, cancellationToken);
                        }
                    })
                ));

                var results = await Task.WhenAll(tasks);
                userResults.AddRange(results);
                tasks.Clear();
            }

            stopwatch.Stop();  // Stop the stopwatch
            Console.WriteLine($"File processing took {stopwatch.ElapsedMilliseconds} ms");

            if (excludedUsers.Any())
            {
                var excludedWorkbook = new XLWorkbook();
                var excludedWorksheet = excludedWorkbook.Worksheets.Add("ExcludedUsers");

                // Set headers
                excludedWorksheet.Cell(1, 1).Value = "Email";
                excludedWorksheet.Cell(1, 2).Value = "DateOfBirth";
                excludedWorksheet.Cell(1, 3).Value = "Groups";
                excludedWorksheet.Cell(1, 4).Value = "Exception";

                // Center the headers and add some formatting
                excludedWorksheet.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                excludedWorksheet.Row(1).Style.Font.Bold = true; // Make headers bold
                excludedWorksheet.Row(1).Style.Fill.BackgroundColor = XLColor.LightGray; // Optional: Add background color

                int row = 2;
                foreach (var excludedUser in excludedUsers)
                {
                    excludedWorksheet.Cell(row, 1).Value = excludedUser.Email;
                    excludedWorksheet.Cell(row, 2).Value = excludedUser.DateOfBirth.ToDateTime(TimeOnly.MinValue);
                    excludedWorksheet.Cell(row, 3).Value = excludedUser.Groups;
                    excludedWorksheet.Cell(row, 4).Value = excludedUser.Exception;

                    // Center the cells for each row
                    excludedWorksheet.Row(row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Add some spacing (optional: set height for better visibility)
                    excludedWorksheet.Row(row).Height = 20; // Adjust height as needed

                    row++;
                }
                // Adjust Column Widths and Finalize Workbook
                excludedWorksheet.Columns().AdjustToContents();

                using (var outputStream = new MemoryStream())
                {
                    excludedWorkbook.SaveAs(outputStream);
                    return outputStream.ToArray(); // Return the Excel file as byte array
                }
            }
            return true;
        }
    }
}