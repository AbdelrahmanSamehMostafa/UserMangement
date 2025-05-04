namespace UserManagment.Common.DTO.Common
{
    public record BaseListingInput
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SearchString { get; set; } = "";
        public virtual string? Sorting { get; set; }
    }
}
