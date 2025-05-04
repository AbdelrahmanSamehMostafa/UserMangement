using UserManagment.Common.DTO.LookUps;
using UserManagment.Common.DTO.UserDTo;

namespace UserManagment.Common.DTO.GroupDTO
{
    public record GroupDetailsDto : GroupListDTO
    {
        public IEnumerable<LookUpDTO>? Roles { get; set; }
        public IEnumerable<UserLookupDto>? Users { get; set; }
    }
}