using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagmentRazor.Helpers.Enums;

namespace com.gbg.modules.utility.Helpers.DTo.UsersDTO
{
    public class UploadImageDto
    {
        public IFormFile file { get; set; }
        public Guid EntityId { get; set; }
        public AttachmentType attachmentType { get; set; } = AttachmentType.User;
    }
}