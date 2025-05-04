using com.gbg.modules.utility.Extentions;
using com.gbg.modules.utility.Helpers.Common.Messages;
using com.gbg.modules.utility.Helpers.DTo.AuthenticationDto;
using com.gbg.modules.utility.Helpers.DTo.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using UserManagmentRazor.Extentions;
using UserManagmentRazor.Helpers.Common.Messages;

namespace com.gbg.modules.utility.Areas.Authentication.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        private readonly IGenericApiService _apiService;

        public LoginModel(ILogger<LoginModel> logger, IGenericApiService apiService)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [BindProperty]
        public LoginDto Input { get; set; } = new LoginDto
        {
            emailAddress = string.Empty,
            password = string.Empty
        };

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Login attempt for email: {emailAddress}", Input.emailAddress);

            var requestDto = new LoginDto
            {
                emailAddress = Input.emailAddress,
                password = Input.password
            };

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors and try again.";
                return Page();
            }


            // Call the API and handle the response using a custom error handler
            var response = await _apiService.LoginAsync<LoginDto, BaseResponse<LoginResponseDto>>(
                APIEndPoint.Login,
                requestDto, null);

            if (response.isSuccess)
            {
                // Store the token in session storage
                HttpContext.Session.SetString("AuthToken", response.data.Token);

                // Extract user details from the token
                var userClaims = JwtHelper.DecodeToken(response.data.Token);
                HttpContext.Session.SetString("UserId", userClaims.UserId);
                HttpContext.Session.SetString("UserName", userClaims.UserName);
                HttpContext.Session.SetString("UserType", userClaims.UserType);
                // Serialize the PoliciesNames list to JSON and store it in session
                var policiesNamesJson = JsonConvert.SerializeObject(userClaims.PoliciesNames);
                HttpContext.Session.SetString("PoliciesNames", policiesNamesJson);

                TempData["SuccessMessage"] = SuccessResponseMessages.Success_login;
                return RedirectToPage("/Index");
            }
            else
            {
                GenericErrorHandling.HandleApiErrorResponse(response, TempData);
                return Page();
            }
        }
    }
}