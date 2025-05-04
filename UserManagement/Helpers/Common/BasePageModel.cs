using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using UserManagmentRazor.Helpers.DTo.PermissionsDto;
using UserManagmentRazor.Helpers.DTo.Breadcrumb;

namespace com.gbg.modules.utility.Helpers.Common
{
    public abstract class BasePageModel : PageModel
    {
        protected string token { get; private set; }
        protected Guid? screenId { get; private set; }
        protected readonly ILogger _logger;
        public List<BreadcrumbItem> Breadcrumbs { get; set; }

        public BasePageModel(ILogger logger)
        {
            _logger = logger;
        }

        public override void OnPageHandlerExecuting(Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutingContext context)
        {
            token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                context.Result = RedirectToPage("/Authentication/AccessDenied");
            }
            var allowedScreensJson = HttpContext.Session.GetString("AllowedScreens");
            if (!string.IsNullOrEmpty(allowedScreensJson))
            {
                var allowedScreens = JsonConvert.DeserializeObject<List<ScreenMenuResponseDto>>(allowedScreensJson);
                var urlSegments = context.HttpContext.Request.Path.Value?.ToLower();
                var regexPattern = @"^/([^/]+)/create/[^/]+$";

                if (Regex.IsMatch(urlSegments, regexPattern))
                {
                    var Module = Regex.Match(urlSegments, regexPattern).Groups[1].Value.ToLower();
                    screenId = allowedScreens?
                        .FirstOrDefault(s => Regex.IsMatch(s.AreaName.ToLower(), regexPattern) && s.AreaName.ToLower().Split('/')[1] == Module)
                        ?.ScreenId;
                }
                else
                {
                    screenId = allowedScreens?
                        .FirstOrDefault(s => s?.AreaName.ToLower() == urlSegments)
                        ?.ScreenId;
                }

                if (screenId == null)
                {
                    TempData["ErrorMessage"] = "You have no permission to access this page";
                    context.Result = Redirect("/Home");
                    return;
                }
                ViewData["ScreenId"] = screenId;
            }


            base.OnPageHandlerExecuting(context);
        }
    }
}