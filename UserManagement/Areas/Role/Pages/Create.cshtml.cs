using com.gbg.modules.utility.Helpers.Common;
using com.gbg.modules.utility.Helpers.Common.Messages;
using com.gbg.modules.utility.Helpers.DTo.Common;
using com.gbg.modules.utility.Helpers.DTo.RolesDTO;
using Microsoft.AspNetCore.Mvc;
using UserManagmentRazor.Extentions;
using UserManagmentRazor.Helpers.Common;
using UserManagmentRazor.Helpers.DTo.Breadcrumb;
using UserManagmentRazor.Helpers.DTo.LookUps;
using UserManagmentRazor.Helpers.DTo.RolesDTO;

namespace UserManagmentRazor.Areas.Role.Pages
{
    public class CreateModel : BasePageModel
    {
        private readonly IGenericApiService _apiService;

        [BindProperty]
        public RoleDto Role { get; set; } = new();

        public bool IsEditMode { get; private set; } = false;

        public string ButtonText => IsEditMode ? "Update" : "Create";

        public CreateModel(ILogger<IndexModel> logger, IGenericApiService apiService) : base(logger)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this, IsEditMode);
            var actionsAllowed = await _apiService.GetAsync<BaseResponse<List<Actions>>>(APIEndPoint.AllowedActions.Replace("{id}", $"{screenId}"), token);
            if (actionsAllowed != null && actionsAllowed.isSuccess) ViewData["AllowedActions"] = actionsAllowed.data;
            if (id.HasValue)
            {
                IsEditMode = true;

                var Response = await _apiService.GetAsync<BaseResponse<RoleDto>>($"{APIEndPoint.Role_GetById}?id={id}", token);

                if (Response.data == null && !Response.isSuccess)
                {
                    TempData["ErrorMessage"] = Response.message;
                    return Page();
                }
                else
                {
                    Role = Response.data;
                    return Page();
                }
            }
            else
            {
                IsEditMode = false;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var actionsAllowed = await _apiService.GetAsync<BaseResponse<List<Actions>>>(APIEndPoint.AllowedActions.Replace("{id}", $"{screenId}"), token);
            if (actionsAllowed != null && actionsAllowed.isSuccess) ViewData["AllowedActions"] = actionsAllowed.data;
            if (!ModelState.IsValid)
            {
                
                return Page();
            }

            if (Role.Id != null && Role.Id != Guid.Empty)
            {
                // edit role
                var RoleToUpdate = new RoleUpdateDto
                {
                    roleDto = Role
                };

                // Attempt to update the role
                var response = await _apiService.PutAsync<RoleUpdateDto, BaseResponse<RoleDto>>(
                    $"{APIEndPoint.Role_Update}", RoleToUpdate, token);

                if (response.isSuccess)
                {
                    // Successful update
                    TempData["SuccessMessage"] = response.message;
                    Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this, IsEditMode);
                    return RedirectToPage("./RoleList");
                }
                else
                {
                    GenericErrorHandling.HandleApiErrorResponse(response, TempData);
                    Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this, IsEditMode);
                    return Page();
                }
            }
            else
            {
                // Create new role
                var response = await _apiService.PostAsync<RoleDto, BaseResponse<RoleDto>>($"{APIEndPoint.Role_Create}", Role, token);
                if (response.isSuccess)
                {
                    // Successful update
                    TempData["SuccessMessage"] = response.message;
                    Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this, IsEditMode);
                    return RedirectToPage("./RoleList");
                }
                else
                {
                    GenericErrorHandling.HandleApiErrorResponse(response, TempData);
                    Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this, IsEditMode);
                    return Page();
                }
            }

        }
    }
}