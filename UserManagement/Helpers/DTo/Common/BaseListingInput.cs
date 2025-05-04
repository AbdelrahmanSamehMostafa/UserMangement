namespace UserManagmentRazor.Helpers.DTo.Common
{
    public record BaseListingInput
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchString { get; set; } = "";
        public virtual string? Sorting { get; set; }
    }
}
