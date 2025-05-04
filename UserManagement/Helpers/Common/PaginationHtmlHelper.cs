using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;

namespace UserManagmentRazor.Helpers
{
    public static class PaginationHtmlHelper
    {
        public static IHtmlContent Paginate<TModel>(this IHtmlHelper htmlHelper, TModel model)
        {  
            return htmlHelper.Partial("_Paging", model);
        }
    }
}
