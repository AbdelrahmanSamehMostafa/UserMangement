using com.gbg.modules.utility.Helpers.Common;
using UserManagmentRazor.Helpers.Common;

namespace UserManagmentRazor.Areas.Role.Pages
{
    public class IndexModel : BasePageModel
    {

        public IndexModel(ILogger<IndexModel> logger) : base(logger)
        {
        }
        public void OnGet()
        {
            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
        }
    }
}
