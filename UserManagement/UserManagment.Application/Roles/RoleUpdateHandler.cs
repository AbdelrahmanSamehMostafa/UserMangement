using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.Commands;
using UserManagment.Application.RoleRecords;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.Roles
{
    public class UpdateRoleHandler : IRequestHandler<UpdateRoleCommand, RoleResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRoleHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RoleResultDto> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var rowFound = await _unitOfWork.Role.GetById(request.roleDto.Id);

            if (rowFound == null)
            {
                throw new CustomException(ErrorResponseMessage.NotFound);
            }

            bool isRepeated = await _unitOfWork.Role.IsFound(request.roleDto);
            
            if (isRepeated)
            {
                throw new CustomException(ErrorResponseMessage.isRepeated);
            }
            var defaultExist = await _unitOfWork.Role.GetDefaultRole();

            if (request.roleDto.IsDefault && defaultExist != null && defaultExist.Id != request.roleDto.Id)
            {
                throw new CustomException(ErrorResponseMessage.Role_Default);
            }

            var res = await _unitOfWork.Role.Update(request.roleDto);
            res.UpdatedDate = DateTime.Now;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var roleResultDto = RoleMapping.ToRoleResult(res);
            return roleResultDto;
        }
    }
}
