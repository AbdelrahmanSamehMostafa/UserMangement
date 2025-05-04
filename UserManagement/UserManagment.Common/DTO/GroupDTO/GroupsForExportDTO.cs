

namespace UserManagment.Common.DTO.GroupDTO
{
    public record GroupsForExportDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int NumberofUsers { get; set; }
        public int NumberofRoles { get; set; }
    }
}