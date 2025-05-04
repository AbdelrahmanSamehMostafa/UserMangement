using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using UserManagmentRazor.Extentions;
using UserManagmentRazor.Helpers.DTo.ConfigurationDTO;
using UserManagmentRazor.Helpers.Common;
using com.gbg.modules.utility.Helpers.Common;
using com.gbg.modules.utility.Helpers.DTo.Common;
using com.gbg.modules.utility.Helpers.Common.Messages;
using UserManagmentRazor.Helpers.Enums;

namespace UserManagmentRazor.Areas.Configuration.Pages;
public class EmailTemplateModel : BasePageModel
{
    private readonly IGenericApiService _apiService;

    [BindProperty]
    public string SelectedTemplateKey { get; set; }

    [BindProperty]
    public string NewTemplateKey { get; set; }

    [BindProperty]
    public string SelectedTemplateBody { get; set; }

    public List<SelectListItem> EmailTemplateKeys { get; set; }

    public EmailTemplateModel(ILogger<EmailTemplateModel> logger, IGenericApiService apiService) : base(logger)
    {
        _apiService = apiService;
    }

    // Handles loading the template on GET
    public async Task<IActionResult> OnGetAsync(string? SelectedTemplateKey)
    {
        // Fetch the available email template keys
        EmailTemplateKeys = await GetEmailTemplateKeysAsync();

        if (!string.IsNullOrEmpty(SelectedTemplateKey) && SelectedTemplateKey != "new-template")
        {
            // Fetch the body for the selected template
            SelectedTemplateBody = (
                await _apiService.GetAsync<BaseResponse<EmailTemplateDto>>($"{APIEndPoint.Config_GetEmailTemplate}{SelectedTemplateKey}",
                 token)).data.ConfigValue;
        }

        Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
        return Page();
    }

    // Handles saving/updating the template on POST
    public async Task<IActionResult> OnPostAsync()
    {
        string newTemp = null;
        if (!string.IsNullOrEmpty(NewTemplateKey))
        {
            // Creating a new template with NewTemplateKey
            var response = await _apiService.PostAsync<UpdateEmailTemplateDto, BaseResponse<bool>>(APIEndPoint.Config_UpdateEmailTemplate,
                new UpdateEmailTemplateDto
                {
                    ConfigKey = NewTemplateKey,
                    ConfigType = ConfigurationType.emailtemplate.ToString(),
                    NewTemplateBody = SelectedTemplateBody
                }, token);
            newTemp = NewTemplateKey;

            if (response.isSuccess && response.data)
            {
                TempData["SuccessMessage"] = response.message;
            }
            else
            {
                GenericErrorHandling.HandleApiErrorResponse(response, TempData);
            }

        }
        else if (!string.IsNullOrEmpty(SelectedTemplateKey) && SelectedTemplateKey != "new-template")
        {
            // Updating an existing template
            var response = await _apiService.PostAsync<UpdateEmailTemplateDto, BaseResponse<bool>>(APIEndPoint.Config_UpdateEmailTemplate,
                new UpdateEmailTemplateDto
                {
                    ConfigKey = SelectedTemplateKey,
                    ConfigType = ConfigurationType.emailtemplate.ToString(),
                    NewTemplateBody = SelectedTemplateBody
                }, token);

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
        return RedirectToPage();
    }

    // Helper method to fetch email template keys
    private async Task<List<SelectListItem>> GetEmailTemplateKeysAsync()
    {
        var response = await _apiService.GetAsync<BaseResponse<List<string>>>(APIEndPoint.Config_GetEmailKeys, token);
        return response.data.Select(k => new SelectListItem(k, k)).ToList();
    }
}
