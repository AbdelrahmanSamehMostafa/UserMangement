using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.DTO.Common;

namespace UserManagment.Application.Group
{
    public record AddUserDto(GroupUserDTO GroupUserDTO) : IRequest<bool>;
    public class AddUserToGroupHandler : IRequestHandler<AddUserDto, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddUserToGroupHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(AddUserDto request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.GroupUser.addGroupUserAsync(request.GroupUserDTO.userId, request.GroupUserDTO.groupId, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return true;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }
    }
}