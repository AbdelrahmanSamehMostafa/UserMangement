using MediatR;
using Microsoft.AspNetCore.Http;
using UserManagment.Common.Enum;

namespace UserManagment.Application.Commands
{
    public class UploadImageCommand : IRequest<Guid>
    {
        public IFormFile file { get; set; }
        public Guid EntityId { get; set; } // The ID of the user associated with the image
        public AttachmentType attachmentType { get; set; } = AttachmentType.User; // The type of the attachment
    }
}