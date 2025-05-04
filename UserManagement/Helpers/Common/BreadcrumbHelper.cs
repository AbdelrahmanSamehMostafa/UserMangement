using Microsoft.AspNetCore.Mvc.RazorPages;
using UserManagmentRazor.Helpers.DTo.Breadcrumb;

namespace UserManagmentRazor.Helpers.Common
{
    public static class BreadcrumbHelper
    {
        public static List<BreadcrumbItem> GenerateBreadcrumb(PageModel pageModel, bool? IsEdit = null)
        {
            List<BreadcrumbItem> breadcrumbs = new List<BreadcrumbItem>();
            var request = pageModel.HttpContext.Request;
            var path = request.Path.Value;

            // Split the path into segments
            var segments = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            // Home breadcrumb
            if (segments.Length == 1 && segments[0].ToLower() == "home" ) 
            {
                breadcrumbs.Add(new BreadcrumbItem
                {
                    Title = "Home",
                    Url = "/Home",
                    IsActive = true
                });
                return breadcrumbs;
            }
            breadcrumbs.Add(new BreadcrumbItem
            {
                Title = "Home",
                Url = "/Home",
                IsActive = segments.Length == 0
            });
            // Dynamically generate breadcrumbs based on URL segments
            string urlAccumulator = string.Empty;
            for (int i = 0; i < segments.Length; i++)
            {
                
                urlAccumulator += "/" + segments[i];
                bool isLastSegment = i == segments.Length - 1;
                if (IsEdit.HasValue)
                {
                    if (IsEdit.Value && segments[i].ToLower() == "create")
                    {
                        breadcrumbs.Add(new BreadcrumbItem
                        {
                            Title = CapitalizeSegment(segments[i]).Replace("Create", "Edit"),
                            Url = urlAccumulator,
                            IsActive = true
                        });
                    }
                    else
                    {
                        breadcrumbs.Add(new BreadcrumbItem
                        {
                            Title = CapitalizeSegment(segments[i]),
                            Url = urlAccumulator,
                            IsActive = isLastSegment
                        });
                    }
                }
                else
                {
                    breadcrumbs.Add(new BreadcrumbItem
                    {
                        Title = CapitalizeSegment(segments[i]),
                        Url = urlAccumulator,
                        IsActive = isLastSegment
                    });
                }
            }

            return breadcrumbs;
        }


        // Capitalizes each segment of the breadcrumb
        private static string CapitalizeSegment(string segment)
        {
            if (string.IsNullOrEmpty(segment))
            {
                return segment;
            }

            // Add spaces before each uppercase letter except the first one
            string result = string.Concat(segment.Select((x, i) => i > 0 && char.IsUpper(x) ? " " + x : x.ToString()));

            // Capitalize the first letter of the result
            return char.ToUpper(result[0]) + result.Substring(1);
        }

    }
}
