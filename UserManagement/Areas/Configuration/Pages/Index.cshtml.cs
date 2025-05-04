using com.gbg.modules.utility.Helpers.Common;
using UserManagmentRazor.Extentions;
using UserManagmentRazor.Helpers.Common;

namespace UserManagmentRazor.Areas.Configuration.Pages
{
    public class IndexModel : BasePageModel
    {
        private readonly IGenericApiService _apiService;

        public IndexModel(ILogger<IndexModel> logger, IGenericApiService apiService) : base(logger)
        {
            _apiService = apiService;
        }

        public void OnGet()
        {
            Breadcrumbs = BreadcrumbHelper.GenerateBreadcrumb(this);
        }
    }
}
