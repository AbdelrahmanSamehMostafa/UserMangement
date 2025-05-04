using UserManagmentRazor.Helpers.DTo.Common;

namespace com.gbg.modules.utility.Helpers.DTo.Common
{
    public record LogsSearchInput : BaseListingInput
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}