using com.gbg.modules.utility.Helpers.Common;
using com.gbg.modules.utility.Helpers.Common.Messages;
using com.gbg.modules.utility.Helpers.DTo.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using UserManagmentRazor.Extentions;
using UserManagmentRazor.Helpers.Common;
using UserManagmentRazor.Helpers.DTo.Breadcrumb;
using UserManagmentRazor.Helpers.DTo.PermissionsDto;

namespace UserManagmentRazor.Pages
{
    // [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IGenericApiService _apiService;
        public List<BreadcrumbItem> Breadcrumbs { get; set; }
        public IndexModel(ILogger<IndexModel> logger, IGenericApiService apiService)
        {
            _apiService = apiService;
        }

        public string UserName { get; set; }
        
        public async Task<IActionResult> OnGetAsync()
        {
            // Access the session in this method instead of the constructor
            var token = HttpContext.Session.GetString("AuthToken");
            if (token != null)
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                UserName = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value ?? "User";

                var response = await _apiService.GetAsync<BaseResponse<List<ScreenMenuResponseDto>>>(APIEndPoint.GetMenu, token);
                if (response.isSuccess)
                {
                    var allowedScreens = response.data;
                    HttpContext.Session.SetString("AllowedScreens", JsonConvert.SerializeObject(allowedScreens));
                    ViewData["AllowedMainScreens"] = allowedScreens.Where(s => s.ParentId == null || s.ParentId == Guid.Empty).ToList();
                    ViewData["AllowedSubScreens"] = allowedScreens.Where(s => s.ParentId != null && s.ParentId != Guid.Empty).ToList();
                }
                else
                {
                    GenericErrorHandling.HandleApiErrorResponse(response, TempData);
                }

            }
            else
            {
                UserName = "Guest";
            }

            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
            return Page();
        }

    }
}
