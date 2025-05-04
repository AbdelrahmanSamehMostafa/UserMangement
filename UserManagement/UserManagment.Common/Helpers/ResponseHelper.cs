using Abp.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace UserManagment.Common.Helpers
{
    public static class ResponseHelper
    {
        public static async Task<IActionResult> HandleRequestAsync<TRequest, TResponse>(
            TRequest request,
            Func<TRequest, Task<TResponse>> action,
            string successMessage,
            string nullMessage = ErrorResponseMessage.NullRequest,
            string notFoundMessage = ErrorResponseMessage.NotFound,
            string isRepeatedMessage = ErrorResponseMessage.isRepeated,
            int badRequestCode = StatusResponsesCode.StatusCodeBadRequest,
            int conflictCode = StatusResponsesCode.StatusCodeConflict,
            int successCode = StatusResponsesCode.StatusCodeOK,
            int notFoundCode = StatusResponsesCode.StatusCodeNotFound,
            int UnauthorizedCode = StatusResponsesCode.StatusCodeUnauthorized,
            int ForbiddenCode = StatusResponsesCode.StatusCodeForbidden)
        {
            var response = new BaseResponse<TResponse>();

            // Check for null or empty Guid
            if (request == null || (request is Guid id && id == Guid.Empty))
            {
                response.IsSuccess = false;
                response.StatusCode = badRequestCode;
                response.Message = nullMessage;
                return new BadRequestObjectResult(response);
            }

            try
            {
                // Perform the action
                var result = await action(request);

                // Check for default value (like null for reference types or false for bools)
                if (EqualityComparer<TResponse>.Default.Equals(result, default))
                {
                    response.IsSuccess = false;
                    response.StatusCode = conflictCode;
                    response.Message = notFoundMessage;
                    return new ConflictObjectResult(response);
                }

                // Success response
                response.IsSuccess = true;
                response.StatusCode = successCode;
                response.Message = successMessage;
                response.Data = result;
                return new OkObjectResult(response);
            }
            catch (CustomException ex) when (ex.Message == ErrorResponseMessage.NotFound)
            {
                response.IsSuccess = false;
                response.StatusCode = notFoundCode;
                response.Message = notFoundMessage;
                return new ConflictObjectResult(response);
            }
            catch (CustomException ex) when (ex.Message == ErrorResponseMessage.Forbidden)
            {
                response.IsSuccess = false;
                response.StatusCode = ForbiddenCode;
                response.Message = ex.Message;
                return new ConflictObjectResult(response);
            }

            catch (CustomException ex) when (ex.Message == ErrorResponseMessage.isRepeated)
            {
                response.IsSuccess = false;
                response.StatusCode = conflictCode;
                response.Message = isRepeatedMessage;
                return new ConflictObjectResult(response);
            }
            catch (CustomException ex)
            {
                response.IsSuccess = false;
                response.StatusCode = badRequestCode;
                response.Message = ex.Message;
                return new ConflictObjectResult(response);
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.StatusCode = StatusResponsesCode.StatusCodeInternalServerError;
                response.Message = ErrorResponseMessage.InternalServerError;
                return new ObjectResult(response) { StatusCode = StatusResponsesCode.StatusCodeInternalServerError };
            }
        }

        public static void SetFileNameHeader(HttpResponse response, string fileName)
        {
            var encodedFileName = Uri.EscapeDataString(fileName);
            response.Headers.Append("FileName", encodedFileName);
            response.Headers.Append("Access-Control-Expose-Headers", "FileName");
        }

        public static (string ContentType, string FileName) GetExcelFileDetails(string fileNamePrefix)
        {
            var fileName = $"{fileNamePrefix}_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return (contentType, fileName);
        }
        public static (DateOnly DateFrom, DateOnly DateTo) GetDateRange(DateOnly? dateFrom, DateOnly? dateTo)
        {
            // Calculate DateFrom
            DateOnly calculatedDateFrom = dateFrom ??
                                          (dateTo.HasValue
                                              ? DateOnly.FromDateTime(dateTo.Value.ToDateTime(TimeOnly.MinValue).StartOfWeek(DayOfWeek.Sunday))
                                              : DateOnly.FromDateTime(DateTime.Now.StartOfWeek(DayOfWeek.Sunday)));

            // Calculate DateTo
            DateOnly calculatedDateTo = dateTo ??
                                        (dateFrom.HasValue
                                            ? DateOnly.FromDateTime(DateTime.Now.StartOfWeek(DayOfWeek.Sunday)).AddDays(6) // End of the current week
                                            : DateOnly.FromDateTime(DateTime.Now));

            return (calculatedDateFrom, calculatedDateTo);
        }

        public static byte[] GetErrorFileBytes(string fileName)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Errors");

                // Add some headers
                worksheet.Cells[1, 1].Value = "Error Message"; // Example header
                worksheet.Cells[1, 2].Value = "Details"; // Example header

                // Add your error details here (this is just a placeholder)
                // You would typically fetch error details from your data source
                var errors = new List<(string Message, string Details)>
        {
            ("Error 1", "Detail 1"),
            ("Error 2", "Detail 2"),
            // Add more errors as needed
        };

                // Populate the worksheet with error data
                for (int i = 0; i < errors.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = errors[i].Message;
                    worksheet.Cells[i + 2, 2].Value = errors[i].Details;
                }

                // Return the byte array of the Excel file
                return package.GetAsByteArray();
            }
        }

    }
}
