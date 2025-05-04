using com.gbg.modules.utility.Helpers.Common;
using com.gbg.modules.utility.Helpers.Common.Messages;
using com.gbg.modules.utility.Helpers.DTo.Common;
using Microsoft.AspNetCore.Mvc;
using UserManagmentRazor.Extentions;
using UserManagmentRazor.Helpers.Common;
using UserManagmentRazor.Helpers.DTo.GroupsDTO;
using UserManagmentRazor.Helpers.DTo.LookUps;

namespace UserManagmentRazor.Areas.Group.Pages
{
    public class ViewModel : BasePageModel
    {
        private readonly IGenericApiService _apiService;

        public GroupResponseDto group { get; set; }

        public ViewModel(ILogger<ViewModel> logger, IGenericApiService apiService) : base(logger)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var response = await _apiService.GetAsync<BaseResponse<GroupResponseDto>>(APIEndPoint.Group_GetById.Replace("id", $"{id}"), token);
            if (response.isSuccess)
            {
                group = response.data;
                TempData["SuccessMessage"] = response.message;
            }
            else
            {
                TempData["ErrorMessage"] = response.message;
                GenericErrorHandling.HandleApiErrorResponse(response, TempData);
            }

            var actionsAllowed = await _apiService.GetAsync<BaseResponse<List<Actions>>>(APIEndPoint.AllowedActions.Replace("{id}", $"{screenId}"), token);
            if (actionsAllowed != null && actionsAllowed.isSuccess) ViewData["AllowedActions"] = actionsAllowed.data;
            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
            return Page();
        }
    }
}
