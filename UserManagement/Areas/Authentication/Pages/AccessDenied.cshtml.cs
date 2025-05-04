using Microsoft.AspNetCore.Mvc.RazorPages;

namespace com.gbg.modules.utility.Areas.Authentication.Pages
{
    public class AccessDeniedModel : PageModel
    {
        private readonly ILogger<AccessDeniedModel> _logger;

        public AccessDeniedModel(ILogger<AccessDeniedModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}