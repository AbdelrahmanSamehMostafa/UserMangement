using com.gbg.modules.utility.Extentions;
using com.gbg.modules.utility.Helpers.Common;
using com.gbg.modules.utility.Helpers.Common.Messages;
using com.gbg.modules.utility.Helpers.DTo.AuthenticationDto;
using com.gbg.modules.utility.Helpers.DTo.Common;
using Microsoft.AspNetCore.Mvc;
using UserManagmentRazor.Extentions;
using UserManagmentRazor.Helpers.Common;

namespace com.gbg.modules.utility.Areas.Authentication.Pages
{
    public class ChangePasswordModel : BasePageModel
    {
        private readonly IGenericApiService _apiService;

        [BindProperty]
        public ChangePasswordDto Input { get; set; }

        public ChangePasswordModel(ILogger<ChangePasswordModel> logger, IGenericApiService apiService) : base(logger)
        {
            _apiService = apiService;
        }

        public void OnGet()
        {
            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            var userClaims = JwtHelper.DecodeToken(token);

            // Ensure the user ID is valid before parsing it to a Guid
            if (Guid.TryParse(userClaims.UserId, out Guid parsedUserId))
            {
                Input.userId = parsedUserId;
            }

            // Call the API using the GenericApiService
            var response = await _apiService.PutAsync<ChangePasswordDto, BaseResponse<bool>>($"{APIEndPoint.Change_Password}", Input, token);
            if (response.isSuccess)
            {
                TempData["SuccessMessage"] = response.message;
            }
            else
            {
                GenericErrorHandling.HandleApiErrorResponse(response, TempData);
            }

            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
            return Page();
        }
    }
}