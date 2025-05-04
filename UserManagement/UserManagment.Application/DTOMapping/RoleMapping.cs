using UserManagment.Common.DTO.ScreenMenu;
using UserManagment.Domain.Models;

namespace UserManagment.Application.RoleRecords
{
    public record RoleResultDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
    }
    public record RoleGetAllResult
    {
        public IEnumerable<RoleResultDto> Roles { get; set; }
        public int Count { get; set; }
    }

    public static class RoleMapping
    {

        public static RoleResultDto ToRoleResult(this Role role)
        {
            var res = new RoleResultDto
            {
                Id = role.Id,
                Name = role.Name,
                IsDefault = role.IsDefault
            };
            return res;
        }

        public static RoleGetAllResult ToRoleGetAllResult(this IEnumerable<RoleResultDto> roles, int count)
        {
            var res = new RoleGetAllResult
            {
                Roles = roles.ToList(),
                Count = count
            };
            return res;
        }

    }
}