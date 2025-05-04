namespace UserManagment.Common.DTO.ScreenMenu
{
    public record ScreenMenuResponseDto
    {
        public required Guid ScreenId { get; set; }
        public required string ScreenName { get; set; }
        public required string AreaName { get; set; }
        public required Guid? ParentId { get; set; }
        public required bool? IsMenuScreen { get; set; }
    }
}