using com.gbg.modules.utility.Helpers.Common;
using com.gbg.modules.utility.Helpers.Common.Messages;
using com.gbg.modules.utility.Helpers.DTo.Common;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using UserManagmentRazor.Extentions;
using UserManagmentRazor.Helpers.Common;
using UserManagmentRazor.Helpers.DTo.Common;
using UserManagmentRazor.Helpers.DTo.LookUps;
using UserManagmentRazor.Helpers.DTo.RolesDTO;

namespace UserManagmentRazor.Areas.Role.Pages
{
    public class RoleListModel : BasePageModel
    {
        private readonly IGenericApiService _apiService;

        // Bind property for search input, SupportsGet ensures it persists in query parameters
        [BindProperty(SupportsGet = true)]
        public string SearchRoleName { get; set; }

        public RoleList Roles { get; set; }

        public RoleListModel(ILogger<RoleListModel> logger, IGenericApiService apiService) : base(logger)
        {
            _apiService = apiService;
            Roles = new RoleList();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var actionsAllowed = await _apiService.GetAsync<BaseResponse<List<Actions>>>(APIEndPoint.AllowedActions.Replace("{id}", $"{screenId ?? Guid.Empty}"), token);
            if (actionsAllowed != null && actionsAllowed.isSuccess) ViewData["AllowedActions"] = actionsAllowed.data;

            int pageNumber = int.TryParse(Request.Query["PageNumber"], out var parsedPageNumber) ? parsedPageNumber : 1;
            int pageSize = int.TryParse(Request.Query["PageSize"], out var parsedPageSize) ? parsedPageSize : 10;

            // Search string is trimmed and converted to lowercase to ensure case-insensitive search
            var requestParams = new BaseListingInput
            {
                SearchString = string.IsNullOrWhiteSpace(SearchRoleName) ? null : SearchRoleName.Trim().ToLower(),  // Case-insensitive search
                Sorting = "CreatedDate desc",
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var response = await _apiService.SearchAsync<string, BaseListingInput, BaseResponse<RoleList>>(
                APIEndPoint.Role_List, requestParams, new BaseResponse<RoleList>(), token, false);

            // Store the request parameters in session
            HttpContext.Session.SetString("RoleSearchParams", JsonSerializer.Serialize(requestParams));

            if (response.isSuccess)
            {
                Roles = response.data;
                Roles.PageNumber = requestParams.PageNumber;
            }
            else
            {
                GenericErrorHandling.HandleApiErrorResponse(response, TempData);
            }

            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid roleId)
        {
            var response = await _apiService.DeleteAsync<BaseResponse<bool>>($"{APIEndPoint.Role_Delete}?roleId={roleId}", token);
            if (response.isSuccess)
            {
                TempData["SuccessMessage"] = response.message;
                Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
                await OnGetAsync();
                return Page();
            }
            else
            {
                TempData["ErrorMessage"] = response.message;
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnGetExportAsync(int PageNumber = 1, int PageSize = 20)
        {
            try
            {
                var requestParams = new BaseListingInput
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    SearchString = string.IsNullOrEmpty(SearchRoleName) ? null : SearchRoleName.Trim(),
                    Sorting = "CreatedDate desc"
                };

                var excelData = await _apiService.ExportExcelAsync(APIEndPoint.Role_ExportToExcel, token, requestParams);

                // Check if the data is null or empty
                if (excelData == null || excelData.Length == 0)
                {
                    TempData["ErrorMessage"] = "Export failed. No data returned.";
                    return RedirectToPage("Index");
                }

                var fileName = $"RoleExport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                var fileStream = new MemoryStream(excelData);

                return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Export error: {ex.Message}");
                TempData["ErrorMessage"] = "Internal server error during export.";
                return RedirectToPage("Index");
            }
        }
    }
}
