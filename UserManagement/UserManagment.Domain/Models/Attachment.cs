using System.ComponentModel.DataAnnotations.Schema;
using UserManagment.Common.Enum;

namespace UserManagment.Domain.Models
{
    [Table("Attachment")]
    public partial class Attachment : BaseModel
    {
        public byte[] Content { get; set; }
        public string Extension { get; set; }
        public Guid? EntityId { get; set; }
        public AttachmentType AttachmentType { get; set; }
        public string FileName { get; set; }
    }
}
