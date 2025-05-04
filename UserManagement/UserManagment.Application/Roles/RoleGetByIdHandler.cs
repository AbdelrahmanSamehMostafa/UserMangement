using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.RoleRecords;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.Roles
{
    public record RoleGetById(Guid id) : IRequest<RoleResultDto>;

    public class RoleGetByIdHandler : IRequestHandler<RoleGetById, RoleResultDto>
    {

        private readonly IUnitOfWork _unitOfWork;

        public RoleGetByIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RoleResultDto> Handle(RoleGetById request, CancellationToken cancellationToken)
        {

            var rowFound = await _unitOfWork.Role.GetById(request.id);
            if (rowFound == null)
            {
                throw new CustomException(ErrorResponseMessage.NotFound);
            }

            var roleResultDto = RoleMapping.ToRoleResult(rowFound);
            return roleResultDto;

        }

    }


}
