using com.gbg.modules.utility.Helpers.Common;
using com.gbg.modules.utility.Helpers.DTo.AuditLog;
using com.gbg.modules.utility.Helpers.DTo.Common;
using Microsoft.AspNetCore.Mvc;
using UserManagmentRazor.Extentions;
using UserManagmentRazor.Helpers.Common;
using UserManagmentRazor.Helpers.DTo.Common;
using UserManagmentRazor.Helpers.DTo.LookUps;

namespace com.gbg.modules.utility.Areas.AuditLogs.Pages
{
    public class IndexModel : BasePageModel
    {
        private readonly IGenericApiService _apiService;

        [BindProperty(SupportsGet = true)]
        public DateTime DateFrom { get; set; } = new DateTime();


        [BindProperty(SupportsGet = true)]
        public DateTime DateTo { get; set; } = new DateTime();

        [BindProperty]
        public AuditLogList AuditLog { get; set; }

        public IndexModel(IGenericApiService apiService, ILogger<IndexModel> logger) : base(logger)
        {
            _apiService = apiService;
            AuditLog = new AuditLogList();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var actionsAllowed = await _apiService.GetAsync<BaseResponse<List<Actions>>>(APIEndPoint.AllowedActions.Replace("{id}", $"{screenId??Guid.Empty}"), token);
            if (actionsAllowed != null && actionsAllowed.isSuccess) ViewData["AllowedActions"] = actionsAllowed.data;
            string url = BuildLogsUrl(APIEndPoint.AuditLog, DateFrom, DateTo);
            int pageNumber = int.TryParse(Request.Query["PageNumber"], out var parsedPageNumber) ? parsedPageNumber : 1;
            int pageSize = int.TryParse(Request.Query["PageSize"], out var parsedPageSize) ? parsedPageSize : 10;

            // Search string is trimmed and converted to lowercase to ensure case-insensitive search
            var requestParams = new LogsSearchInput
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                DateFrom = DateFrom,
                DateTo = DateTo
            };

            // Call the API service to get the audit log
            var response = await _apiService.SearchAsync<string, BaseListingInput, BaseResponse<AuditLogList>>(
                           url, requestParams, new BaseResponse<AuditLogList>(), token, true);
            if (response.isSuccess)
            {
                AuditLog = response.data;
                AuditLog.PageNumber = requestParams.PageNumber;
                // Sort the logs by Date and Time with the correct format
                AuditLog.auditLogs = AuditLog.auditLogs.OrderBy(log => DateTime.ParseExact(log.Date + " " + log.Time, "yyyy-MM-dd HH:mm:ss.FFFFFFF", null)).ToList();
                TempData["SuccessMessage"] = response.message;
            }

            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
            return Page();
        }


        public async Task<IActionResult> OnGetExportAsync()
        {

            // Format the dates as 'MM-dd-yyyy' for the API URL
            string url = BuildLogsUrl(APIEndPoint.AuditLog_Export, DateFrom, DateTo);

            // Call the API service to get the access log
            var excelData = await _apiService.ExportExcelAsync(url, token);

            // Check if the data is null or empty
            if (excelData == null || excelData.Length == 0)
            {
                TempData["ErrorMessage"] = "Export failed. No data returned.";
                return RedirectToPage();
            }

            var fileName = $"AuditLogExport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var fileStream = new MemoryStream(excelData);

            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
            return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public static string BuildLogsUrl(string baseUrl, DateTime dateFrom, DateTime dateTo)
        {
            var url = baseUrl;

            var queryParams = new List<string>();

            // Check if the user has modified the DateFrom and DateTo values
            if (dateFrom != default(DateTime))
            {
                queryParams.Add($"DateFrom={dateFrom:yyyy-MM-dd}");
            }

            if (dateTo != default(DateTime))
            {
                queryParams.Add($"DateTo={dateTo:yyyy-MM-dd}");
            }

            // If there are query parameters, append them to the URL
            if (queryParams.Any())
            {
                url += "?" + string.Join("&", queryParams);
            }

            return url;
        }
    }
}