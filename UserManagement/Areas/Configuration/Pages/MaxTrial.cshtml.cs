using Microsoft.AspNetCore.Mvc;
using UserManagmentRazor.Extentions;
using UserManagmentRazor.Helpers.DTo.ConfigurationDTO;
using UserManagmentRazor.Helpers.Common;
using com.gbg.modules.utility.Helpers.Common;
using com.gbg.modules.utility.Helpers.DTo.Common;
using com.gbg.modules.utility.Helpers.Common.Messages;
namespace UserManagmentRazor.Areas.Configuration.Pages;
public class MaxTrialsLoginModel : BasePageModel
{
    private readonly IGenericApiService _apiService;

    [BindProperty]
    public bool IsActivated { get; set; }

    [BindProperty]
    public MaxTrialDto MaxTrial { get; set; }

    public MaxTrialsLoginModel(IGenericApiService apiService, ILogger<MaxTrialsLoginModel> logger) : base(logger)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var response = await _apiService.GetAsync<BaseResponse<MaxTrialDto>>(APIEndPoint.Config_MaxTrial, token);

        if (response.isSuccess)
        {
            TempData["SuccessMessage"] = response.message;
            MaxTrial = response.data;
            if (MaxTrial.MaxDurationInMinutes == 0 && MaxTrial.MaxTrial == 0)
            {
                IsActivated = false;
            }
            else
            {
                IsActivated = true;
            }
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
        if (IsActivated && !ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Please ensure all fields are filled correctly.";
            return Page();
        }

        var config = new MaxTrialDto
        {
            MaxDurationInMinutes = IsActivated ? MaxTrial.MaxDurationInMinutes : 0,
            MaxTrial = IsActivated ? MaxTrial.MaxTrial : 0
        };

        // Save the configuration via the backend API
        var response = await _apiService.PostAsync<MaxTrialDto, BaseResponse<bool>>(APIEndPoint.Config_MaxTrial, config, token);
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
