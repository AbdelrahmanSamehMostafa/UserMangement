using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using UserManagmentRazor.Extentions;
using UserManagmentRazor.Helpers.DTo.Common;
using com.gbg.modules.utility.Helpers.DTo.UsersDTO;
using com.gbg.modules.utility.Helpers.DTo.AuthenticationDto;
using com.gbg.modules.utility.Helpers.DTo.Common;
using UserManagmentRazor.Helpers.Common;
using UserManagmentRazor.Helpers.Enums;
using com.gbg.modules.utility.Helpers.Common;
using com.gbg.modules.utility.Helpers.Common.Messages;
using System.Text;
using UserManagmentRazor.Helpers.DTo.LookUps;

namespace UserManagmentRazor.Areas.User.Pages
{
    public class IndexModel : BasePageModel
    {
        private readonly IGenericApiService _apiService;

        private readonly string templateFileName = "ImportUsersTemplateFile.xlsx";

        [BindProperty(SupportsGet = true)]
        public string SearchUserName { get; set; }

        [BindProperty(SupportsGet = true)]
        public LockStatus LockStatus { get; set; }

        public UserList usersList { get; set; } = new UserList();

        public IndexModel(ILogger<IndexModel> logger, IGenericApiService apiService) : base(logger)
        {
            _apiService = apiService;
            usersList = new UserList();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            int pageNumber = int.TryParse(Request.Query["PageNumber"], out var parsedPageNumber) ? parsedPageNumber : 1;
            int pageSize = int.TryParse(Request.Query["PageSize"], out var parsedPageSize) ? parsedPageSize : 10;

            var actionsAllowed = await _apiService.GetAsync<BaseResponse<List<Actions>>>(APIEndPoint.AllowedActions.Replace("{id}", $"{screenId??Guid.Empty}"), token);
            if (actionsAllowed != null && actionsAllowed.isSuccess) ViewData["AllowedActions"] = actionsAllowed.data;

            var requestParams = new UserListingInput
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchString = string.IsNullOrEmpty(SearchUserName) ? null : SearchUserName.Trim(),
                Sorting = "CreatedDate desc", // Sort users by creation date
                LockStatus = LockStatus // Add lock status to filter
            };

            // Fetch the filtered list of users
            var response = await _apiService.SearchAsync<string, UserListingInput, BaseResponse<UserList>>(
                APIEndPoint.User_List, requestParams, new BaseResponse<UserList>(), token, false);

            // Store the request parameters in session
            HttpContext.Session.SetString("UserSearchParams", JsonSerializer.Serialize(requestParams));

            if (response.isSuccess)
            {
                usersList = response.data;
                usersList.PageNumber = requestParams.PageNumber;
            }
            else
            {
                GenericErrorHandling.HandleApiErrorResponse(response, TempData);
            }

            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid userId)
        {
            var response = await _apiService.DeleteAsync<BaseResponse<bool>>($"{APIEndPoint.User_Delete}{userId}", token);
            if (response.isSuccess)
            {
                TempData["SuccessMessage"] = response.message;
            }
            else
            {
                GenericErrorHandling.HandleApiErrorResponse(response, TempData);
            }

            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPost(string userEmail, string? handler)
        {
            if (handler == null)
            {
                return RedirectToPage("Index", new { ScreenId = screenId });
            }
            var result = await _apiService.PutAsync<ResetPasswordDto, BaseResponse<bool>>(APIEndPoint.ResetPassword,
            new ResetPasswordDto { Email = userEmail }, token);
            if (result.isSuccess)
            {
                TempData["SuccessMessage"] = result.message;
            }
            else
            {
                GenericErrorHandling.HandleApiErrorResponse(result, TempData);
            }

            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnGetExportToExcelAsync(int PageNumber = 1, int PageSize = 20)
        {
            try
            {
                // Retrieve the stored request parameters from session
                var searchParamsJson = HttpContext.Session.GetString("UserSearchParams");
                UserListingInput requestParams = null;

                if (!string.IsNullOrEmpty(searchParamsJson))
                {
                    requestParams = JsonSerializer.Deserialize<UserListingInput>(searchParamsJson);
                }
                else
                {
                    // Fallback in case session is empty
                    requestParams = new UserListingInput
                    {
                        PageNumber = PageNumber,
                        PageSize = PageSize,
                        SearchString = string.IsNullOrEmpty(SearchUserName) ? null : SearchUserName.Trim(),
                        Sorting = "CreatedDate desc",
                        LockStatus = LockStatus
                    };
                }

                var excelData = await _apiService.ExportExcelAsync(APIEndPoint.User_ExportToExcel, token, requestParams);

                // Check if the data is null or empty
                if (excelData == null || excelData.Length == 0)
                {
                    TempData["ErrorMessage"] = "Export failed. No data returned.";
                    return RedirectToPage("Index");
                }

                HttpContext.Session.Remove("UserSearchParams");

                var fileName = "UserExport.xlsx";
                var fileStream = new MemoryStream(excelData);

                return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Export error: {ex.Message}");
                TempData["ErrorMessage"] = "Error during export.";
                return RedirectToPage("Index");
            }
        }

        public async Task<IActionResult> OnPostUploadTemplateAsync(IFormFile templateFile)
        {
            if (templateFile == null || templateFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Please upload a valid file.";
                return RedirectToPage("Index", new { ScreenId = screenId });
            }

            // Check if the uploaded file's name matches the expected template file name
            if (Path.GetFileName(templateFile.FileName) != templateFileName)
            {
                TempData["ErrorMessage"] = "Invalid file name. Please upload the correct template.";
                return RedirectToPage("Index", new { ScreenId = screenId });
            }


            // Create the UserMultipleInsertDto with the uploaded file
            var userMultipleInsertDto = new UserMultipleInsertDto
            {
                File = templateFile
            };

            // Call the generic UploadFileAsync method to send the file to the backend API
            var response = await _apiService.UploadFileAsync<UserMultipleInsertDto, BaseResponse<object>>(
                APIEndPoint.user_multiple_insert,
                userMultipleInsertDto, token);

            // Check the API response
            if (response.isSuccess)
            {
                TempData["SuccessMessage"] = response.message;
            }
            else if (!response.isSuccess)
            {
                TempData["ErrorMessage"] = response.message;

                var response2 = await _apiService.ExportExcelAsync(response.data.ToString(), token);
                var fileName = "UserExport.xlsx";

                string tempFilePath = Path.Combine(Path.GetTempPath(), fileName);

                // Save the file to a temporary location
                await System.IO.File.WriteAllBytesAsync(tempFilePath, response2);
                // Store the file path in TempData or a session variable for retrieval

                TempData["FilePath"] = tempFilePath;
                HttpContext.Session.SetString("ExportedFilePath", tempFilePath);
                TempData["DownloadMessage"] = "An export file has been created and is ready for download.";
            }
            else
            {
                GenericErrorHandling.HandleApiErrorResponse(response, TempData);
            }

            return RedirectToPage("Index", new { ScreenId = screenId });
        }

        public IActionResult OnGetDownloadExportedFile()
        {
            // Retrieve the file path from the session
            var filePath = HttpContext.Session.GetString("ExportedFilePath");

            if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
            {
                var fileName = Path.GetFileName(filePath);

                // Read the file into memory and serve it for download
                var fileBytes = System.IO.File.ReadAllBytes(filePath);

                // Optionally delete the file after download
                System.IO.File.Delete(filePath);
                HttpContext.Session.Remove("ExportedFilePath");

                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }

            TempData["ErrorMessage"] = "The file is no longer available for download.";
            return RedirectToPage("Index");
        }


        public async Task<IActionResult> OnPostLockAsync(Guid Id)
        {
            var result = await _apiService.PutAsync<Guid, BaseResponse<bool>>(APIEndPoint.Lock_User.Replace("{id}", $"{Id}"),
                Id, token);
            if (result.isSuccess)
            {
                TempData["SuccessMessage"] = result.message;
            }
            else
            {
                GenericErrorHandling.HandleApiErrorResponse(result, TempData);
            }

            await OnGetAsync();
            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
            return Page();
        }

        public async Task<IActionResult> OnPostUnlockAsync(Guid Id)
        {
            var result = await _apiService.PutAsync<Guid, BaseResponse<bool>>(APIEndPoint.Unlock_User.Replace("{id}", $"{Id}"),
                Id, token);
            if (result.isSuccess)
            {
                TempData["SuccessMessage"] = result.message;
            }
            else
            {
                GenericErrorHandling.HandleApiErrorResponse(result, TempData);
            }

            await OnGetAsync();
            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
            return Page();
        }
    }
}
