using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.DTO.UserDTo;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.Users
{
    // Request for getting a user by Id
    public record GetUserById(Guid Id) : IRequest<UserDetailsDTO?>;

    // Handler for the GetUserById
    public class UserGetByIdHandler : IRequestHandler<GetUserById, UserDetailsDTO?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserGetByIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserDetailsDTO?> Handle(GetUserById request, CancellationToken cancellationToken)
        {

            var user = await _unitOfWork.Authentication.GetUserByIdAsync(request.Id, cancellationToken);
            if (user == null)
            {
                throw new CustomException(ErrorResponseMessage.User_NotFound);
            }
            user.Groups = await _unitOfWork.GroupUser.GetUserGroupsAsync(request.Id, cancellationToken);
            user.Image = await _unitOfWork.Attachment.GetUserImageContentByUserId(request.Id);
            return user;
        }
    }
}