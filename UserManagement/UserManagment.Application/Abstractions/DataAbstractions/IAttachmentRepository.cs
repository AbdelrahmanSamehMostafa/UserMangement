using System.Runtime.CompilerServices;
using UserManagment.Common.Enum;
using UserManagment.Domain.Models;

namespace UserManagment.Application.Abstractions.DataAbstractions
{
    public interface IAttachmentRepository
    {
        Task<Guid?> Create(Attachment request);
        Task<bool> CheckForUserImg(Guid userId);
        Task DeleteUserImg(Guid userId, AttachmentType attachmentType);
        Task<byte[]?> GetUserImageContentByUserId(Guid userId);
        IAsyncEnumerable<byte[]?> GetUserImageContentByUserIdsAsync(IEnumerable<Guid> userIds, [EnumeratorCancellation] CancellationToken cancellationToken);
    }
}