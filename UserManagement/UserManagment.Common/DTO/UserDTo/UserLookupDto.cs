using UserManagment.Common.DTO.LookUps;

namespace UserManagment.Common.DTO.UserDTo
{
    public record UserLookupDto : LookUpDTO
    {
        public string Email { get; set; }
    }
}