using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.Group
{
    public record GroupDeleteByIdDTO(Guid id) : IRequest<bool>;

    public class GroupDeleteByIdHandler : IRequestHandler<GroupDeleteByIdDTO, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GroupDeleteByIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(GroupDeleteByIdDTO request, CancellationToken cancellationToken)
        {
            var group = await _unitOfWork.Group.GetById(request.id);
            if (group == null)
            {
               throw new CustomException(ErrorResponseMessage.Group_NotFound);
            }
            group.IsDeleted = true;
            group.DeletedDate = DateTime.Now;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

}