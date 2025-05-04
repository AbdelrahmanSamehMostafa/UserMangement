namespace com.gbg.modules.utility.Helpers.DTo.ProfileDTO
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public List<GroupDto> groups { get; set; }
        public byte[] Image { get; set; }
    }
}