using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.DTO.Common;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.Users
{
    public record AddGroupsDto(UserGroupsDTO UserGroupsDTO) : IRequest<bool>;
    public class AddUserToGroupHandler : IRequestHandler<AddGroupsDto, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddUserToGroupHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(AddGroupsDto request, CancellationToken cancellationToken)
        {
            if (!request.UserGroupsDTO.groupIds.Any())
            {
                throw new CustomException(ErrorResponseMessage.NoGroups);
            }
            await _unitOfWork.GroupUser.addGroupsUserAsync(request.UserGroupsDTO.userId, request.UserGroupsDTO.groupIds, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;


        }
    }
}