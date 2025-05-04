using com.gbg.modules.utility.Helpers.Common;
using com.gbg.modules.utility.Helpers.Common.Messages;
using com.gbg.modules.utility.Helpers.DTo.Common;
using com.gbg.modules.utility.Helpers.DTo.ConfigurationDTO;
using Microsoft.AspNetCore.Mvc;
using UserManagmentRazor.Extentions;
using UserManagmentRazor.Helpers.Common;
using UserManagmentRazor.Helpers.Enums;

namespace com.gbg.modules.utility.Areas.Configuration.Pages
{
    public class MailConfigurationModel : BasePageModel
    {
        private readonly IGenericApiService _apiService;

        [BindProperty]
        public List<ConfigurationDto> Configurations { get; set; } = new();

        public MailConfigurationModel(ILogger<MailConfigurationModel> logger, IGenericApiService apiService) : base(logger)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var response = await _apiService.GetListAsync<BaseResponse<List<ConfigurationDto>>>(APIEndPoint.Config_Mail_Get, token);
            if (response.isSuccess)
            {
                Configurations = response.data;
                TempData["SuccessMessage"] = response.message;
            }
            else
            {
                GenericErrorHandling.HandleApiErrorResponse(response, TempData);
            }

            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            foreach (var config in Configurations)
            {
                config.ConfigType = ConfigurationType.email.ToString();
            }

            // Validate the Configurations list
            if (Configurations == null || !Configurations.Any())
            {
                TempData["ErrorMessage"] = "No configurations to update.";
                return Page();
            }

            // Call the API to update the configurations
            var response = await _apiService.PutAsync<List<ConfigurationDto>, BaseResponse<bool>>(APIEndPoint.Config_Mail_Post, Configurations, token);
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