using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.Roles
{

    public record DeleteRole(Guid Id) : IRequest<bool>;

    public class RoleDeleteHandler : IRequestHandler<DeleteRole, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleDeleteHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteRole request, CancellationToken cancellationToken)
        {

            var res = await _unitOfWork.Role.GetById(request.Id);
            if (res == null)
            {
               throw new CustomException(ErrorResponseMessage.NotFound);
            }
            if (res.IsDefault) 
            {
                throw new CustomException(ErrorResponseMessage.DefaultRole);
            }

            res.DeletedDate = DateTime.Now;
            res.IsDeleted = true;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;

        }

    }


}
