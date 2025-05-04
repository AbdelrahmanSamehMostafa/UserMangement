using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.Exceptions;
using UserManagment.Application.RoleRecords;
using UserManagment.Common.DTO.Common;
using UserManagment.Common.Helpers;
using UserManagment.Common.Messages;

namespace UserManagment.Application.Roles
{
    public record RoleGetAll(BaseListingInput baseListingInput) : IRequest<RoleGetAllResult>;

    public class RoleGetAllHandler : IRequestHandler<RoleGetAll, RoleGetAllResult>
    {

        private readonly IUnitOfWork _unitOfWork;

        public RoleGetAllHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RoleGetAllResult> Handle(RoleGetAll request, CancellationToken cancellationToken)
        {
            var res = await _unitOfWork.Role.GetRolesAsync(request.baseListingInput, cancellationToken);
            if (!res.Rolses.Any())
            {
                throw new CustomException(ErrorResponseMessage.NotFound);
            }

            var paginatedRoles = res.Rolses.Skip((request.baseListingInput.PageNumber - 1) * request.baseListingInput.PageSize)
                .Take(request.baseListingInput.PageSize)
                .ToList();

            var data = RoleMapping.ToRoleGetAllResult(paginatedRoles, res.Count);

            return data;
        }


    }


}
