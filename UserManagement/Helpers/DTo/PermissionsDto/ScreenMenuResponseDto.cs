namespace UserManagmentRazor.Helpers.DTo.PermissionsDto
{
    public class ScreenMenuResponseDto
    {
        public Guid ScreenId { get; set; }
        public string ScreenName { get; set; }
        public string AreaName { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsMenuScreen { get; set; }
    }
}
