using com.gbg.modules.utility.Helpers.Common;
using com.gbg.modules.utility.Helpers.Common.Messages;
using com.gbg.modules.utility.Helpers.DTo.Common;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using UserManagmentRazor.Extentions;
using UserManagmentRazor.Helpers.Common;
using UserManagmentRazor.Helpers.DTo.Breadcrumb;
using UserManagmentRazor.Helpers.DTo.Common;
using UserManagmentRazor.Helpers.DTo.GroupsDTO;
using UserManagmentRazor.Helpers.DTo.LookUps;

namespace UserManagmentRazor.Areas.Group.Pages
{
    public class IndexModel : BasePageModel
    {
        private readonly IGenericApiService _apiService;
        public List<BreadcrumbItem> Breadcrumbs { get; set; }
        [BindProperty]
        public string SearchGroupName { get; set; } // Added for group name search

        public GroupList Groups { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IGenericApiService apiService) : base(logger)
        {
            _apiService = apiService;
            Groups = new GroupList();
        }

        public async Task<IActionResult> OnGetAsync(string? searchGroupName = null)
        {
            int pageNumber = int.TryParse(Request.Query["PageNumber"], out var parsedPageNumber) ? parsedPageNumber : 1;
            int pageSize = int.TryParse(Request.Query["PageSize"], out var parsedPageSize) ? parsedPageSize : 10;

            var actionsAllowed = await _apiService.GetAsync<BaseResponse<List<Actions>>>(APIEndPoint.AllowedActions.Replace("{id}", $"{screenId ?? Guid.Empty}"), token);
            if (actionsAllowed != null && actionsAllowed.isSuccess) ViewData["AllowedActions"] = actionsAllowed.data;

            var queryParams = new BaseListingInput
            {
                SearchString = searchGroupName, // Add search criteria
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var response = await _apiService.SearchAsync<BaseListingInput, BaseListingInput, BaseResponse<GroupList>>(
                APIEndPoint.Group_List, queryParams, new BaseResponse<GroupList>(), token, false);

            // Store the request parameters in session
            HttpContext.Session.SetString("GroupSearchParams", JsonSerializer.Serialize(queryParams));

            if (response.isSuccess)
            {
                Groups = response.data;
                Groups.PageNumber = queryParams.PageNumber;
            }
            else
            {
                GenericErrorHandling.HandleApiErrorResponse(response, TempData);
            }

            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            var response = await _apiService.DeleteAsync<BaseResponse<bool>>($"{APIEndPoint.Group_Delete}{id}", token);
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
                return Page();
            }
        }

        public async Task<IActionResult> OnGetExportToExcelAsync(int PageNumber = 1, int PageSize = 20)
        {
            try
            {
                // Retrieve the stored request parameters from session
                var searchParamsJson = HttpContext.Session.GetString("GroupSearchParams");
                BaseListingInput requestParams = null;

                if (!string.IsNullOrEmpty(searchParamsJson))
                {
                    requestParams = JsonSerializer.Deserialize<BaseListingInput>(searchParamsJson);
                }
                else
                {
                    // Fallback in case session is empty
                    requestParams = new BaseListingInput
                    {
                        PageNumber = PageNumber,
                        PageSize = PageSize,
                        SearchString = string.IsNullOrEmpty(SearchGroupName) ? null : SearchGroupName.Trim(),
                        Sorting = "CreatedDate desc"
                    };
                }

                var excelData = await _apiService.ExportExcelAsync(APIEndPoint.Group_ExportToExcel, token, requestParams);

                // Check if the data is null or empty
                if (excelData == null || excelData.Length == 0)
                {
                    TempData["ErrorMessage"] = "Export failed. No data returned.";
                    return RedirectToPage("Index");
                }

                HttpContext.Session.Remove("GroupSearchParams");

                var fileName = "GroupExport.xlsx";
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
