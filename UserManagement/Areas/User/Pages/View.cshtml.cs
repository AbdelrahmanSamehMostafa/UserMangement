using com.gbg.modules.utility.Helpers.Common;
using com.gbg.modules.utility.Helpers.Common.Messages;
using com.gbg.modules.utility.Helpers.DTo.AuthenticationDto;
using com.gbg.modules.utility.Helpers.DTo.Common;
using com.gbg.modules.utility.Helpers.DTo.UsersDTO;
using Microsoft.AspNetCore.Mvc;
using UserManagmentRazor.Extentions;
using UserManagmentRazor.Helpers.Common;
using UserManagmentRazor.Helpers.DTo.Breadcrumb;
using UserManagmentRazor.Helpers.DTo.LookUps;

namespace com.gbg.modules.utility.Areas.User.Pages
{
    public class ViewModel : BasePageModel
    {
        private readonly IGenericApiService _apiService;

        public UserViewDto user { get; set; }

        public ViewModel(ILogger<ViewModel> logger, IGenericApiService apiService) : base(logger)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var response = await _apiService.GetAsync<BaseResponse<UserViewDto>>($"{APIEndPoint.User_GetById}{id}", token);
            if (response.isSuccess)
            {
                TempData["SuccessMessage"] = response.message;
                user = response.data;
            }
            else
            {
                GenericErrorHandling.HandleApiErrorResponse(response, TempData);
            }

            var actionsAllowed = await _apiService.GetAsync<BaseResponse<List<Actions>>>(APIEndPoint.AllowedActions.Replace("{id}", $"{screenId}"), token);
            if (actionsAllowed != null && actionsAllowed.isSuccess) ViewData["AllowedActions"] = actionsAllowed.data;
            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string email)
        {
            if (!ModelState.IsValid)
            {
                var actionsAllowed = await _apiService.GetAsync<BaseResponse<List<Actions>>>(APIEndPoint.AllowedActions.Replace("{id}", $"{screenId}"), token);
                if (actionsAllowed != null && actionsAllowed.isSuccess) ViewData["AllowedActions"] = actionsAllowed.data;
                return Page();
            }

            // Ensure the email is populated correctly from the form
            var Input = new ResetPasswordDto
            {
                Email = email
            };

            // Call the API using the GenericApiService
            var response = await _apiService.PutAsync<ResetPasswordDto, BaseResponse<bool>>($"{APIEndPoint.Reset_Password}", Input, token);

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
    }
}
