using com.gbg.modules.utility.Helpers.Common;
using com.gbg.modules.utility.Helpers.Common.Messages;
using com.gbg.modules.utility.Helpers.DTo.Common;
using Microsoft.AspNetCore.Mvc;
using UserManagmentRazor.Extentions;
using UserManagmentRazor.Helpers.Common;
using UserManagmentRazor.Helpers.DTo.ConfigurationDTO;

namespace UserManagmentRazor.Areas.Configuration.Pages
{
    public class DomainFormatModel : BasePageModel
    {
        private readonly IGenericApiService _apiService;

        public List<string> AllowedDomains { get; set; } = new List<string>();

        [BindProperty]
        public string newDomains { get; set; }

        [BindProperty]
        public string domainToDelete { get; set; }

        public DomainFormatModel(ILogger<DomainFormatModel> logger, IGenericApiService apiService) : base(logger)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var response = await _apiService.GetAsync<BaseResponse<string>>(APIEndPoint.Config_Domains_Get, token);
            if (response.isSuccess)
            {
                AllowedDomains = response.data.Split(";").Where(d => !string.IsNullOrEmpty(d)).ToList();  // Remove empty entries
                TempData["SuccessMessage"] = response.message;
            }
            else
            {
                GenericErrorHandling.HandleApiErrorResponse(response, TempData);
            }

            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string action)
        {
            // Fetch current allowed domains
            var Domainsresponse = await _apiService.GetAsync<BaseResponse<string>>(APIEndPoint.Config_Domains_Get, token);
            AllowedDomains = Domainsresponse.data.Split(";").Where(d => !string.IsNullOrEmpty(d)).ToList();

            if (action == "add" && !string.IsNullOrEmpty(newDomains))
            {
                var newDomainsArray = newDomains.Split(',').Select(d => d.Trim()).Where(d => !string.IsNullOrEmpty(d)).ToList();

                // Prepare the list of domains for the API call
                var updatedDomains = AllowedDomains.Concat(newDomainsArray).Distinct().ToList();
                var allowedHosts = string.Join(";", updatedDomains);

                // Prepare the payload
                var payload = new DomainFormatDto
                {
                    AllowedDomains = allowedHosts
                };

                // Send the API request to update the domains
                var response = await _apiService.PostAsync<DomainFormatDto, BaseResponse<bool>>(APIEndPoint.Config_Domains_Post, payload, token);

                if (response.isSuccess && response.data)
                {
                    // Update the frontend only if the API call was successful
                    AllowedDomains = updatedDomains;
                    TempData["SuccessMessage"] = response.message;
                }
                else
                {
                    GenericErrorHandling.HandleApiErrorResponse(response, TempData);
                }
            }
            else if (action == "delete" && !string.IsNullOrEmpty(domainToDelete))
            {
                AllowedDomains.Remove(domainToDelete);

                var allowedHosts = string.Join(";", AllowedDomains);

                var payload = new DomainFormatDto
                {
                    AllowedDomains = allowedHosts
                };

                var response = await _apiService.PostAsync<DomainFormatDto, BaseResponse<bool>>(APIEndPoint.Config_Domains_Post, payload, token);

                if (response.isSuccess && response.data)
                {
                    TempData["SuccessMessage"] = response.message;
                }
                else
                {
                    GenericErrorHandling.HandleApiErrorResponse(response, TempData);
                }
            }

            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
            return Page();
        }
    }
}
