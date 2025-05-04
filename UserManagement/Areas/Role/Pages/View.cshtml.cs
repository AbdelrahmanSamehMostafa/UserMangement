using com.gbg.modules.utility.Helpers.Common;
using com.gbg.modules.utility.Helpers.Common.Messages;
using com.gbg.modules.utility.Helpers.DTo.Common;
using Microsoft.AspNetCore.Mvc;
using UserManagmentRazor.Extentions;
using UserManagmentRazor.Helpers.Common;
using UserManagmentRazor.Helpers.DTo.Breadcrumb;
using UserManagmentRazor.Helpers.DTo.LookUps;
using UserManagmentRazor.Helpers.DTo.RolesDTO;

namespace UserManagmentRazor.Areas.Role.Pages
{
    public class ViewModel : BasePageModel
    {
        private readonly IGenericApiService _apiService;

        public BaseResponse<RoleDto> Response { get; set; }

        public ViewModel(ILogger<ViewModel> logger, IGenericApiService apiService) : base(logger)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {


            Response = await _apiService.GetAsync<BaseResponse<RoleDto>>($"{APIEndPoint.Role_GetById}?id={id}", token);

            if (Response.isSuccess)
            {
                // Successful
                TempData["SuccessMessage"] = Response.message;
            }
            else
            {
                GenericErrorHandling.HandleApiErrorResponse(Response, TempData);
            }

            var actionsAllowed = await _apiService.GetAsync<BaseResponse<List<Actions>>>(APIEndPoint.AllowedActions.Replace("{id}", $"{screenId}"), token);
            if (actionsAllowed != null && actionsAllowed.isSuccess) ViewData["AllowedActions"] = actionsAllowed.data;
            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
            TempData["SuccessMessage"] = Response.message;
            return Page();
        }

    }
}
