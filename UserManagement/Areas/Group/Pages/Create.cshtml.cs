using com.gbg.modules.utility.Helpers.Common;
using com.gbg.modules.utility.Helpers.Common.Messages;
using com.gbg.modules.utility.Helpers.DTo.Common;
using com.gbg.modules.utility.Helpers.DTo.GroupsDTO;
using Microsoft.AspNetCore.Mvc;
using UserManagmentRazor.Extentions;
using UserManagmentRazor.Helpers.Common;
using UserManagmentRazor.Helpers.DTo.Common;
using UserManagmentRazor.Helpers.DTo.GroupsDTO;
using UserManagmentRazor.Helpers.DTo.LookUps;
using UserManagmentRazor.Helpers.DTo.RolesDTO;

namespace UserManagmentRazor.Areas.Group.Pages
{
    public class CreateModel : BasePageModel
    {
        private readonly IGenericApiService _apiService;

        [BindProperty]
        public List<Guid> SelectedRoleIds { get; set; } = new();

        [BindProperty]
        public GroupResponseDto Group { get; set; } = new GroupResponseDto();

        [BindProperty]
        public List<RoleDto> Roles { get; set; } = new();

        static bool IsEditMode = false;

        public string Text => IsEditMode ? "Update" : "Create";

        public CreateModel(ILogger<CreateModel> logger, IGenericApiService apiService) : base(logger)
        {
            _apiService = apiService;
        }

        public async Task OnGet(Guid? id)
        {
            // Get all roles
            var actionsAllowed = await _apiService.GetAsync<BaseResponse<List<Actions>>>(APIEndPoint.AllowedActions.Replace("{id}", $"{screenId}"), token);
            if (actionsAllowed != null && actionsAllowed.isSuccess) ViewData["AllowedActions"] = actionsAllowed.data;
            await GetAllRolesAsync();
            if (id.HasValue)
            {
                IsEditMode = true;
                var response = await _apiService.GetAsync<BaseResponse<GroupResponseDto>>(APIEndPoint.Group_GetById.Replace("id", $"{id.Value}"), token);
                if (response.isSuccess)
                {
                    foreach (var role in response.data.roles)
                    {
                        SelectedRoleIds.Add(role.Id);
                        response.data.RolesIds.Add(role.Id);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = response.message;
                    GenericErrorHandling.HandleApiErrorResponse(response, TempData);
                }

                Group = response.data;
            }
            else
            {
                IsEditMode = false;
            }
            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this, IsEditMode);
        }

        private GroupUpdateDto MapGroupDtoToGroupUpdateDto(GroupResponseDto gorupDto)
        {
            return new GroupUpdateDto
            {
                Id = gorupDto.Id,
                Name = gorupDto.Name,
                Code = gorupDto.Code,
                Description = gorupDto.Description,
                RolesIds = gorupDto.RolesIds,
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var actionsAllowed = await _apiService.GetAsync<BaseResponse<List<Actions>>>(APIEndPoint.AllowedActions.Replace("{id}", $"{screenId}"), token);
            if (actionsAllowed != null && actionsAllowed.isSuccess) ViewData["AllowedActions"] = actionsAllowed.data;
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while saving the group.");
                Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this, IsEditMode);
                return Page();
            }


            if (Group.Id != null && Group.Id != Guid.Empty)
            {
                //edit group
                var oldRoleIds = Group.RolesIds ?? new List<Guid>();
                var allRoleIds = oldRoleIds.Union(SelectedRoleIds).ToList();

                Group.RolesIds = allRoleIds;
                var grouprToUpdate = MapGroupDtoToGroupUpdateDto(Group);

                // Check if any groups are selected
                if (SelectedRoleIds == null || !SelectedRoleIds.Any())
                {
                    ModelState.AddModelError("SelectedRoleIds", "Group must have at least one role.");
                    TempData["ErrorMessage"] = "Group must have at least one role.";
                    Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this, IsEditMode = true);
                    await OnGet(Group.Id);
                    return Page();
                }

                var response = await _apiService.PutAsync<GroupUpdateDto, BaseResponse<bool>>($"{APIEndPoint.Group_Update}", grouprToUpdate, token);
                if (response.isSuccess)
                {
                    // Successful update
                    TempData["SuccessMessage"] = response.message;
                    Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this, IsEditMode);
                    return RedirectToPage("./Index");
                }
                else
                {
                    GenericErrorHandling.HandleApiErrorResponse(response, TempData);
                    Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this, IsEditMode);
                    await OnGet(Group.Id);
                    return Page();
                }
            }
            else
            {
                // create group
                Group.RolesIds = SelectedRoleIds;
                // Check if any groups are selected
                if (SelectedRoleIds == null || !SelectedRoleIds.Any())
                {
                    ModelState.AddModelError("SelectedRoleIds", "Group must have at least one role.");
                    TempData["ErrorMessage"] = "Group must have at least one role.";
                    Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this, IsEditMode = false);
                    await OnGet(null);
                    return Page();
                }
                var groupToCreate = new GroupDto
                {
                    Group = Group
                };

                var response = await _apiService.PostAsync<GroupDto, BaseResponse<object>>(APIEndPoint.Group_Create, groupToCreate, token);
                if (response.isSuccess)
                {
                    // Successful update
                    TempData["SuccessMessage"] = response.message;
                    Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this, IsEditMode);
                    return RedirectToPage("./Index");
                }
                else
                {
                    GenericErrorHandling.HandleApiErrorResponse(response, TempData);
                    Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this, IsEditMode);
                    await OnGet(null);
                    return Page();
                }
            }
        }

        private async Task GetAllRolesAsync()
        {
            var requestParams = new BaseListingInput
            {
                PageNumber = 1,
                PageSize = int.MaxValue,
                SearchString = null,
                Sorting = null
            };

            // Call the SearchAsync method
            var response = await _apiService.SearchAsync<string, BaseListingInput, BaseResponse<RoleList>>(
            APIEndPoint.Role_List, requestParams, new BaseResponse<RoleList>(), token, false);

            if (response.data != null && response.isSuccess)
            {
                Roles = response.data.roles;
            }
            else
            {
                GenericErrorHandling.HandleApiErrorResponse(response, TempData);
            }
        }
    }
}
