using com.gbg.modules.utility.Helpers.Common;
using com.gbg.modules.utility.Helpers.Common.Messages;
using com.gbg.modules.utility.Helpers.DTo.Common;
using Microsoft.AspNetCore.Mvc;
using UserManagmentRazor.Extentions;
using UserManagmentRazor.Helpers.Common;
using UserManagmentRazor.Helpers.DTo.ConfigurationDTO;

namespace UserManagmentRazor.Areas.Configuration.Pages
{
    public class PasswordExpirationModel : BasePageModel
    {
        private readonly IGenericApiService _apiService;

        [BindProperty]
        public bool IsActivated { get; set; }

        [BindProperty]
        public int PasswordExpirationPeriod { get; set; } = 0; // Default value is 6 months

        public PasswordExpirationModel(IGenericApiService apiService, ILogger<PasswordExpirationModel> logger) : base(logger)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var response = await _apiService.GetAsync<BaseResponse<PasswordExpirationDto>>(APIEndPoint.Config_PassExpire, token);
            if (response.isSuccess)
            {
                PasswordExpirationPeriod = response.data.MaxDurationInMonth;
                IsActivated = response.data.MaxDurationInMonth != 0; // If default, it's not activated
                TempData["SuccessMessage"] = response.message;
            }
            else
            {
                PasswordExpirationPeriod = 0; // Default
                IsActivated = false;
                GenericErrorHandling.HandleApiErrorResponse(response, TempData);
            }

            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (IsActivated && !ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Fill the required Field, please";
                return Page();
            }

            var config = new PasswordExpirationDto
            {
                MaxDurationInMonth = IsActivated ? PasswordExpirationPeriod : 0
            };

            var response = await _apiService.PostAsync<PasswordExpirationDto, BaseResponse<bool>>(APIEndPoint.Config_PassExpire, config, token);
            if (response.isSuccess && response.data)
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
