using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.Commands;
using UserManagment.Common.Enum;
using UserManagment.Common.Helpers;
using UserManagment.Domain.Models;

namespace UserManagment.Application.Users
{
    public class UploadImageCommandHandler : IRequestHandler<UploadImageCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UploadImageCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(UploadImageCommand request, CancellationToken cancellationToken)
        {
            if (request.file == null || request.file.Length == 0)
            {
                throw new CustomException(ErrorResponseMessage.NullRequest);
            }
            bool ishasImage = await _unitOfWork.Attachment.CheckForUserImg(request.EntityId);

            if (ishasImage)
            {
                await _unitOfWork.Attachment.DeleteUserImg(request.EntityId, request.attachmentType);
            }

            using var memoryStream = new MemoryStream();
            await request.file.CopyToAsync(memoryStream);
            var content = memoryStream.ToArray();

            var attachment = new Attachment
            {
                Content = content,
                Extension = Path.GetExtension(request.file.FileName),
                FileName = request.file.FileName,
                EntityId = request.EntityId,
                AttachmentType = AttachmentType.User
            };

            await _unitOfWork.Attachment.Create(attachment);
            var user = await _unitOfWork.Authentication.GetUserProfileAsync(request.EntityId, cancellationToken);
            if (user == null)
            {
                throw new CustomException(ErrorResponseMessage.User_NotFound);
            }
            user.UserImageId = attachment.Id;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return attachment.Id;
        }
    }
}
