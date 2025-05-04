using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.Roles
{
    public record ScreenRoleActionGet(Guid RoleId) : IRequest<List<Guid>?>;
    public class ScreenRoleActionGetHandler : IRequestHandler<ScreenRoleActionGet, List<Guid>?>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ScreenRoleActionGetHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Guid>?> Handle(ScreenRoleActionGet request, CancellationToken cancellationToken)
        {
            var Data = await _unitOfWork.RoleScreenAction.GetRoleScreenActions(request.RoleId, cancellationToken);
            if (Data==null||!Data.Any())
            {
                throw new CustomException(ErrorResponseMessage.NotFound);
            }
            return Data;
        }
    }
}
