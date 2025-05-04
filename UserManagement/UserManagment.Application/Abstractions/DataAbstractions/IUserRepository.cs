using UserManagment.Application.Identity;
using UserManagment.Common.DTO.UserDTo;
using UserManagment.Domain.Models;

namespace UserManagment.Application.Abstractions.DataAbstractions;

public interface IUserRepository
{
    Task<Dictionary<string, bool>> CheckEmailsInBulk(List<string> emails);
    Task<UserDetailsDTO?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<User?> FindUserByIdAsync(Guid id);
    Task<User?> Create(User request);
    Task<User?> GetByEmailAsync(Login request, CancellationToken cancellationToken);
    Task<bool> Update(User request);
    Task<(IEnumerable<UserListDto> Users, int Count)> GetUsersAsync(UserSearchInput baseListingInput, CancellationToken cancellationToken);
    Task<bool> IsFound(string Email, Guid Id);
    Task<User?> GetUserByMailAsync(string Email, CancellationToken cancellationToken);
    Task<User?> GetUserProfileAsync(Guid id, CancellationToken cancellationToken);
}