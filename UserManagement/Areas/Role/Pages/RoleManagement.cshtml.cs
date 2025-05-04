using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UserManagmentRazor.Extentions;
using UserManagmentRazor.Helpers.DTo.LookUps;
using UserManagmentRazor.Helpers.DTo.RolesDTO;
using UserManagmentRazor.Helpers.Common;
using com.gbg.modules.utility.Helpers.Common;
using com.gbg.modules.utility.Helpers.DTo.Common;
using com.gbg.modules.utility.Helpers.Common.Messages;
using UserManagmentRazor.Helpers.DTo.PermissionsDto;

namespace UserManagmentRazor.Areas.Role.Pages
{
    public class RoleManagementModel : BasePageModel
    {
        private readonly IGenericApiService _apiService;

        [BindProperty(SupportsGet = true)] // SupportsGet allows us to bind this property on a GET request
        public Guid SelectedRoleId { get; set; }

        [BindProperty]
        public List<ScreenActionDTO> ScreenActions { get; set; }

        [BindProperty]
        public List<SelectListItem> RolesDropDown { get; set; }

        [BindProperty]
        public List<Guid> RolePermissions { get; set; } = new List<Guid>();

        public RoleManagementModel(IGenericApiService apiService, ILogger<RoleManagementModel> logger) : base(logger)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Load roles for dropdown
            var RolesResponse = await _apiService.GetAsync<BaseResponse<RoleList>>(APIEndPoint.Role_List, token);
            if (RolesResponse.isSuccess)
            {
                RolesDropDown = RolesResponse.data.roles.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name }).ToList();
            }
            else
            {
                GenericErrorHandling.HandleApiErrorResponse(RolesResponse, TempData);
            }

            // Load screens and actions
            var ScreenActionsresposne = await _apiService.GetAsync<BaseResponse<List<ScreenActionDTO>>>(APIEndPoint.LookUp_GetScreens, token);
            if (ScreenActionsresposne.isSuccess)
            {
                ScreenActions = ScreenActionsresposne.data;
            }
            else
            {
                GenericErrorHandling.HandleApiErrorResponse(ScreenActionsresposne, TempData);
            }

            // If a role is selected, load its permissions
            if (SelectedRoleId != Guid.Empty)
            {
                var RolePermissionsResponse = await _apiService.GetAsync<BaseResponse<List<Guid>>>(
                    $"{APIEndPoint.GetScreenActionsOfRole}{SelectedRoleId}", token);
                if (RolePermissionsResponse.isSuccess)
                {
                    RolePermissions = RolePermissionsResponse.data;
                }
                else
                {
                    GenericErrorHandling.HandleApiErrorResponse(RolePermissionsResponse, TempData);
                }
            }
            else
            {
                RolePermissions = new List<Guid>(); // No permissions if no role is selected
            }

            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check if no role is selected
            if (SelectedRoleId == Guid.Empty)
            {
                TempData["ErrorMessage"] = "Please select a role before saving.";
                Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
                return RedirectToPage("/Index");
            }

            if (!ModelState.IsValid)
            {
                Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
                return Page();
            }

            // Extract selected screenActionIds from the form data
            var selectedScreenActionIds = new List<Guid>();

            foreach (var key in Request.Form.Keys)
            {
                if (key.StartsWith("permissions["))
                {
                    var parts = key.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 3 && parts[0] == "permissions")
                    {
                        if (Guid.TryParse(parts[2], out Guid parsedActionId))
                        {
                            selectedScreenActionIds.Add(parsedActionId);
                        }
                    }
                }
            }

            // Prepare the request payload
            var requestPayload = new AddScreenActionsToRoleDto
            {
                roleId = SelectedRoleId,
                screenActionIds = selectedScreenActionIds
            };


            var response = await _apiService.PostAsync<AddScreenActionsToRoleDto, BaseResponse<bool>>(APIEndPoint.AddScreenActionsToRole, requestPayload, token);
            if (response.isSuccess && response.data)
            {
                TempData["SuccessMessage"] = response.message;
                Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
                //await OnGetAsync();
                return Redirect("/Home");
            }
            else
            {
                Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
                TempData["ErrorMessage"] = response.message;
                await OnGetAsync();
                return Page();
            }
        }
    }
}
