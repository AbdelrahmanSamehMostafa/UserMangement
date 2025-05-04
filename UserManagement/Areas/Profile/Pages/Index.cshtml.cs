using com.gbg.modules.utility.Extentions;
using com.gbg.modules.utility.Helpers.Common;
using com.gbg.modules.utility.Helpers.Common.Messages;
using com.gbg.modules.utility.Helpers.DTo.Common;
using com.gbg.modules.utility.Helpers.DTo.ProfileDTO;
using com.gbg.modules.utility.Helpers.DTo.UsersDTO;
using Microsoft.AspNetCore.Mvc;
using UserManagmentRazor.Extentions;
using UserManagmentRazor.Helpers.Common;
using UserManagmentRazor.Helpers.Enums;

namespace com.gbg.modules.utility.Areas.Profile.Pages
{
    public class UserProfileModel : BasePageModel
    {
        private readonly IGenericApiService _apiService;

        public UserProfileDto UserProfile { get; set; }

        public UserProfileModel(ILogger<UserProfileModel> logger, IGenericApiService apiService) : base(logger)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Decode the token to get the user ID
            var userClaims = JwtHelper.DecodeToken(token);
            var userId = userClaims.UserId;

            _logger.LogInformation($"Fetching profile for user ID: {userId}");

            // Make the API call with the user ID
            var response = await _apiService.GetAsync<BaseResponse<UserProfileDto>>($"{APIEndPoint.User_GetById}{userId}", token);
            if (response.isSuccess)
            {
                UserProfile = response.data;
                TempData["SuccessMessage"] = response.message;
            }
            else
            {
                GenericErrorHandling.HandleApiErrorResponse(response, TempData);
            }

            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
            return Page();
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            var isLoggedOut = await _apiService.LogoutAsync(APIEndPoint.Logout, token);

            if (isLoggedOut)
            {
                HttpContext.Session.Remove("AuthToken");
                HttpContext.Session.Remove("AllowedScreens");
                TempData["SuccessMessage"] = "User logged out successfully.";
                return RedirectToPage("/Login", new { area = "Authentication" });
            }
            else
            {
                TempData["ErrorMessage"] = "Error while logging out";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostUploadImageAsync(IFormFile file, Guid EntityId)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a file to upload.";
                return Page();
            }

            var uploadImageCommand = new UploadImageDto
            {
                file = file,
                EntityId = EntityId,
                attachmentType = AttachmentType.User
            };

            var response = await _apiService.UploadFileAsync<UploadImageDto, BaseResponse<Guid>>(APIEndPoint.User_UploadImage, uploadImageCommand, token);
            if (response.isSuccess)
            {
                TempData["SuccessMessage"] = response.message;
            }
            else
            {
                GenericErrorHandling.HandleApiErrorResponse(response, TempData);
            }

            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
            await OnGetAsync();
            return Page();
        }
    }
}