using UserManagment.Application.Users;
using UserManagment.Common.DTO.UserDTo;
using UserManagment.Domain.Models;

namespace UserManagment.Application.DTOMapping
{
    public record UserResultDto
    {
        public Guid Id { get; init; }

        public string FirstName { get; init; } = string.Empty;

        public string LastName { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public string UserName { get; init; } = string.Empty;

        public DateTime? PasswordLastUpdatedDate { get; init; }

        public string? MobileNumber { get; init; }

        public DateTime DateOfBirth { get; init; }

        public string AddressLocation { get; init; } = string.Empty;

        public string? UserImage { get; init; }

        public bool IsActive { get; init; }

        public bool IsLocked { get; init; }

        // You may add additional properties if you have further information you want to expose, 
        // such as computed properties like FullName if needed.
        public string FullName => $"{FirstName} {LastName}";
    }
    public record UserGetAllResult
    {
        public IEnumerable<UserBaseListDto> Users { get; set; }
        public int Count { get; set; }
    }

    public static class UserMapping
    {

        public static UserResultDto ToUserResult(this User user)
        {
            var result = new UserResultDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                PasswordLastUpdatedDate = user.PasswordLastUpdatedDate,
                MobileNumber = user.MobileNumber,
                DateOfBirth = user.DateOfBirth,
                AddressLocation = user.AddressLocation,
                IsActive = user.IsActive,
                IsLocked = user.IsLocked
            };
            return result;
        }
        public static UserListDto ToUserListDto(this User user)
        {
            return new UserListDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                MobileNumber = user.MobileNumber,
                DateOfBirth = DateOnly.FromDateTime(user.DateOfBirth), // Only year, month, and day
                UserName = user.UserName,
                Location = user.AddressLocation,
                IsLocked = user.IsLocked
            };
        }

        public static User ToUser(this UserRequestDTO request)
        {
            return new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                MobileNumber = request.MobileNumber,
                DateOfBirth = request.DateOfBirth,
                AddressLocation = request.Location,
                IsActive = false,
                IsLocked = false,
                InsertedDate = DateTime.UtcNow
            };
        }
        public static User UserForUpdate(this UserForUpdateDTO request, User user)
        {
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.MobileNumber = request.MobileNumber;
            user.DateOfBirth = request.DateOfBirth;
            user.AddressLocation = request.Location;
            return user;
        }

        public static UserDetailsDTO ToUserDetails(this User user)
        {
            var result = new UserDetailsDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                MobileNumber = user.MobileNumber,
                DateOfBirth = user.DateOfBirth,
                Location = user.AddressLocation,
                Username = user.UserName,
            };
            return result;
        }
        public static UserGetAllResult ToUserGetAllResult(this IEnumerable<UserBaseListDto> users, int count)
        {
            var res = new UserGetAllResult
            {
                Users = users.ToList(),
                Count = count
            };
            return res;
        }

    }

}