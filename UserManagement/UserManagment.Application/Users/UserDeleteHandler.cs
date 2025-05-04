using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.Users
{
    // Request to delete a user
    public record DeleteUser(Guid Id) : IRequest<bool>;

    // Handler for the DeleteUser
    public class UserDeleteHandler : IRequestHandler<DeleteUser, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserDeleteHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteUser request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Authentication.FindUserByIdAsync(request.Id);
            if (user == null)
            {
                throw new CustomException(ErrorResponseMessage.User_NotFound);
            }
            user.IsDeleted = true;
            user.DeletedDate = DateTime.Now;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}