using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.Enum;
using UserManagment.Common.Helpers;
using UserManagment.Domain.Models;

namespace UserManagment.Infrastructure.Repositories
{
    public class AttachmentRepository(UserManagmentDbContext ctx) : IAttachmentRepository
    {
        public async Task<Guid?> Create(Attachment request)
        {
            var res = await ctx.Attachments.AddAsync(request);
            return res.Entity.Id;
        }

        public Task<bool> CheckForUserImg(Guid userId)
        {
            return ctx.Attachments.AnyAsync(x => x.EntityId == userId && x.AttachmentType == AttachmentType.User);
        }

        public async Task DeleteUserImg(Guid userId, AttachmentType attachmentType)
        {
            var attachment = await ctx.Attachments.FirstOrDefaultAsync(x => x.EntityId == userId && x.AttachmentType == AttachmentType.User);

            if (attachment != null && (attachment.AttachmentType == AttachmentType.User || attachment.AttachmentType == AttachmentType.Group))
            {
                ctx.Attachments.Remove(attachment);
            }
            else if (attachment != null)
            {
                attachment.IsDeleted = true;
            }
            else
            {
                throw new CustomException(ErrorResponseMessage.NotFound);
            }
        }

        public async Task<byte[]?> GetUserImageContentByUserId(Guid userId)
        {
            var attachment = await ctx.Attachments
                .AsNoTracking()
                .Where(x => x.EntityId == userId && x.AttachmentType == AttachmentType.User && !x.IsDeleted)
                .Select(x => x.Content)
                .FirstOrDefaultAsync();

            return attachment;
        }

        public async IAsyncEnumerable<byte[]?> GetUserImageContentByUserIdsAsync(IEnumerable<Guid> userIds, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            foreach (var userId in userIds)
            {
                var imageContent = await ctx.Attachments
                    .AsNoTracking()
                    .Where(x => x.EntityId == userId && x.AttachmentType == AttachmentType.User && !x.IsDeleted)
                    .Select(x => x.Content)
                    .FirstOrDefaultAsync(cancellationToken);

                yield return imageContent; // This will return null if no image is found
            }
        }

    }
}