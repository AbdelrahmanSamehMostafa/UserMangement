using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.Identity
{
    public record LockUserCommand(Guid UserId) : IRequest<bool>;

    public class UserLockHandler : IRequestHandler<LockUserCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserLockHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(LockUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var User = await _unitOfWork.Authentication.FindUserByIdAsync(request.UserId);
                if (User == null)
                {
                    throw new CustomException(ErrorResponseMessage.User_NotFound);
                }
                User.IsLocked = true;
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
