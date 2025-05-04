using com.gbg.modules.utility.Helpers.Common;
using com.gbg.modules.utility.Helpers.Common.Messages;
using com.gbg.modules.utility.Helpers.DTo.Common;
using com.gbg.modules.utility.Helpers.DTo.UsersDTO;
using Microsoft.AspNetCore.Mvc;
using UserManagmentRazor.Extentions;
using UserManagmentRazor.Helpers.Common;
using UserManagmentRazor.Helpers.DTo.Common;
using UserManagmentRazor.Helpers.DTo.GroupsDTO;
using UserManagmentRazor.Helpers.DTo.LookUps;

namespace UserManagmentRazor.Areas.User.Pages
{
    public class CreateModel : BasePageModel
    {
        private readonly IGenericApiService _apiService;

        [BindProperty]
        public UserDto User { get; set; } = new();

        [BindProperty]
        public List<Guid> SelectedGroupIds { get; set; } = new();

        [BindProperty]
        public List<GroupWithCounts> Groups { get; set; } = new();

        public bool IsEditMode { get; private set; } = false;

        public string ButtonText => IsEditMode ? "Update" : "Create";

        public CreateModel(ILogger<IndexModel> logger, IGenericApiService apiService) : base(logger)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            // Get all groups
            await GetAllGroupsAsync();
            if (id.HasValue)
            {
                IsEditMode = true;
                var response = await _apiService.GetAsync<BaseResponse<UserDto>>($"{APIEndPoint.User_GetById}{id.Value}", token);
                foreach (var group in response.data.groups)
                {
                    SelectedGroupIds.Add(group.Id);
                    response.data.GroupIds.Add(group.Id);
                }

                if (response.isSuccess)
                {
                    User = response.data;
                }
                else
                {
                    GenericErrorHandling.HandleApiErrorResponse(response, TempData);
                }
            }
            else
            {
                IsEditMode = false;
            }
            var actionsAllowed = await _apiService.GetAsync<BaseResponse<List<Actions>>>(APIEndPoint.AllowedActions.Replace("{id}", $"{screenId}"), token);
            if (actionsAllowed != null && actionsAllowed.isSuccess) ViewData["AllowedActions"] = actionsAllowed.data;
            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this, IsEditMode);
            return Page();
        }

        private UserUpdateDto MapUserDtoToUserUpdateDto(UserDto userDto)
        {
            return new UserUpdateDto
            {
                userDto = new UserToUpdateDto
                {
                    Id = userDto.Id,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    UserName = userDto.UserName,
                    Email = userDto.Email,
                    PasswordLastUpdatedDate = userDto.PasswordLastUpdatedDate,
                    MobileNumber = userDto.MobileNumber,
                    DateOfBirth = userDto.DateOfBirth,
                    Location = userDto.Location,
                    GroupIds = userDto.GroupIds
                }
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var actionsAllowed = await _apiService.GetAsync<BaseResponse<List<Actions>>>(APIEndPoint.AllowedActions.Replace("{id}", $"{screenId}"), token);
                if (actionsAllowed != null && actionsAllowed.isSuccess) ViewData["AllowedActions"] = actionsAllowed.data;
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while saving the user.");
                Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this, IsEditMode);
                return Page();
            }


            if (User.Id != null && User.Id != Guid.Empty)
            {
                //edit user
                var oldGroupIds = User.GroupIds ?? new List<Guid>();
                var allGroupIds = oldGroupIds.Union(SelectedGroupIds).ToList();

                User.GroupIds = allGroupIds;
                var userToUpdate = MapUserDtoToUserUpdateDto(User);

                // Check if any groups are selected
                if (SelectedGroupIds == null || !SelectedGroupIds.Any())
                {
                    ModelState.AddModelError("SelectedGroupIds", "User must have at least one group.");
                    TempData["ErrorMessage"] = "User must have at least one group.";
                    Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this, IsEditMode = true);
                    await OnGetAsync(User.Id);
                    return Page();
                }

                var response = await _apiService.PutAsync<UserUpdateDto, BaseResponse<bool>>($"{APIEndPoint.user_update}",
                    userToUpdate, token);
                if (response.isSuccess)
                {
                    TempData["SuccessMessage"] = response.message;
                    Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this, IsEditMode);
                    return RedirectToPage("./Index");
                }
                else
                {
                    GenericErrorHandling.HandleApiErrorResponse(response, TempData);
                    Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this, IsEditMode);
                    await OnGetAsync(User.Id);
                    return Page();
                }
            }

            else
            {
                // Create new user
                User.GroupIds = SelectedGroupIds;
                // Check if any groups are selected
                if (SelectedGroupIds == null || !SelectedGroupIds.Any())
                {
                    ModelState.AddModelError("SelectedGroupIds", "User must have at least one group.");
                    TempData["ErrorMessage"] = "User must have at least one group.";
                    Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this, IsEditMode = false);
                    await OnGetAsync(null);
                    return Page();
                }

                var response = await _apiService.PostAsync<UserDto, BaseResponse<UserDto>>($"{APIEndPoint.User_Create}", User, token);
                if (response.isSuccess)
                {
                    TempData["SuccessMessage"] = response.message;
                    Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this, IsEditMode);
                    return RedirectToPage("./Index");
                }
                else
                {
                    GenericErrorHandling.HandleApiErrorResponse(response, TempData);
                    Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this, IsEditMode);
                    await OnGetAsync(null);
                    return Page();
                }
            }
        }

        private async Task GetAllGroupsAsync()
        {
            var requestParams = new BaseListingInput
            {
                PageNumber = 1,
                PageSize = int.MaxValue,
                SearchString = null,
                Sorting = null
            };

            var groupsResponse = await _apiService.SearchAsync<GroupList, BaseListingInput, BaseResponse<GroupList>>(
                APIEndPoint.Group_List, requestParams, new BaseResponse<GroupList>(), token, false);
            if (groupsResponse.isSuccess)
            {
                Groups = groupsResponse.data.Groups;
            }
            else
            {
                GenericErrorHandling.HandleApiErrorResponse(groupsResponse, TempData);
            }
        }
    }
}