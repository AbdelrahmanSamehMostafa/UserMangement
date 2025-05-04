
using com.gbg.modules.utility.Helpers.Common.Messages;
using com.gbg.modules.utility.Helpers.DTo.AuthenticationDto;
using com.gbg.modules.utility.Helpers.DTo.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserManagmentRazor.Extentions;

namespace com.gbg.modules.utility.Areas.Authentication.Pages
{
    public class ForgetPasswordModel : PageModel
    {
        private readonly ILogger<ForgetPasswordModel> _logger;
        private readonly IGenericApiService _apiService;

        public ForgetPasswordModel(ILogger<ForgetPasswordModel> logger, IGenericApiService apiService)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [BindProperty]
        public ResetPasswordDto Input { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Call the API using the GenericApiService
            var response = await _apiService.PutAsync<ResetPasswordDto, BaseResponse<bool>>($"{APIEndPoint.Forget_Password}", Input, null);
            if (response.isSuccess)
            {
                TempData["SuccessMessage"] = response.message;
            }
            else
            {
                GenericErrorHandling.HandleApiErrorResponse(response, TempData);
            }

            return Page();
        }
    }
}