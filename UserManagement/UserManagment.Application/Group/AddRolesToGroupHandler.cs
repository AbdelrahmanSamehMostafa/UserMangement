using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.DTO.Common;

namespace UserManagment.Application.Group
{
    public record AddGroupRoleDto(GroupRoleDTO GroupUserDTO) : IRequest<bool>;

    public class AddRolesToGroupHandler : IRequestHandler<AddGroupRoleDto, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddRolesToGroupHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(AddGroupRoleDto request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.GroupRole.AddRange(request.GroupUserDTO.GroupId, request.GroupUserDTO.RolesId, cancellationToken);
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