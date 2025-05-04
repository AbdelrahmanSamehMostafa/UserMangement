using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.Identity
{
    public record UnlockUserCommand(Guid UserId) : IRequest<bool>;

    public class UnlockUserHandler : IRequestHandler<UnlockUserCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UnlockUserHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(UnlockUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var User = await _unitOfWork.Authentication.FindUserByIdAsync(request.UserId);
                if (User == null)
                {
                    throw new CustomException(ErrorResponseMessage.User_NotFound);
                }
                User.IsLocked = false;
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