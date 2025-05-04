using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.DTOMapping;
using UserManagment.Common.DTO.UserDTo;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.Users
{
    // Request for getting list of users
    public record DisplayUsers(UserSearchInput ListingInput) : IRequest<UserGetAllResult>;

    // Handler for the GetUsers
    public class UserGetAllHandler : IRequestHandler<DisplayUsers, UserGetAllResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserGetAllHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserGetAllResult> Handle(DisplayUsers request,
        CancellationToken cancellationToken)
        {
            var _data = await _unitOfWork.Authentication.GetUsersAsync(request.ListingInput, cancellationToken);
            if (!_data.Users.Any())
            {
                throw new CustomException(ErrorResponseMessage.NotFound);
            }

            var userList = _data.Users.ToList();
            var userIds = userList.Select(u => u.Id).ToList();

            // Create an asynchronous stream to fetch user images
            var userImages = new List<byte[]?>();
            await foreach (var userImage in _unitOfWork.Attachment.GetUserImageContentByUserIdsAsync(userIds, cancellationToken))
            {
                userImages.Add(userImage);
            }

            List<UserBaseListDto> users = new List<UserBaseListDto>();
            for (int i = 0; i < userList.Count; i++)
            {
                var item = userList[i];
                var userImage = userImages[i]; // This could be null if no image was found

                users.Add(new UserBaseListDto
                {
                    Id = item.Id,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Email = item.Email,
                    MobileNumber = item.MobileNumber,
                    Location = item.Location,
                    IsLocked = item.IsLocked,
                    Image = userImage 
                });
            }

            var paginatedUsers = users.Skip((request.ListingInput.PageNumber - 1) * request.ListingInput.PageSize)
                .Take(request.ListingInput.PageSize)
                .ToList();

            var data = UserMapping.ToUserGetAllResult(paginatedUsers, _data.Count);

            return data;
        }
    }


}