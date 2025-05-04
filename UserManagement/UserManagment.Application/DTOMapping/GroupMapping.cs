using UserManagment.Common.DTO.GroupDTO;

namespace UserManagment.Application.GroupRecords
{
    public record GroupResultDto
    {
        public Guid Id;
        public string Name;
        public string Code;
        public string Description;
    }
    public record GroupGetAllResult
    {
        public IEnumerable<GroupListDTO> Groups { get; set; }
        public int Count { get; set; }
    }

    public static class GroupMapping
    {
        public static GroupResultDto ToGroupResult(this Domain.Models.Group group)
        {
            return new GroupResultDto
            {
                Id = group.Id,
                Name = group.Name,
                Code = group.Code,
                Description = group.Description
            };
        }

        public static GroupGetAllResult ToGroupGetAllResult(this IEnumerable<GroupListDTO> groups, int count)
        {
            var res = new GroupGetAllResult
            {
                Groups = groups.ToList(),
                Count = count
            };
            return res;
        }
    }
}