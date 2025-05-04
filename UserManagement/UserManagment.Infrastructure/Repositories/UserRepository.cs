using Abp.Collections.Extensions;
using Microsoft.EntityFrameworkCore;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.DTOMapping;
using UserManagment.Application.Identity;
using UserManagment.Common.DTO.UserDTo;
using UserManagment.Common.Enum;
using UserManagment.Domain.Models;

namespace UserManagment.Infrastructure.Repositories
{
    public class UserRepository(UserManagmentDbContext context) : IUserRepository
    {
        public async Task<User?> GetByEmailAsync(Login request, CancellationToken cancellationToken)
        {
            var result = await context.Users.Where(e => e.Email == request.EmailAddress)
                  .FirstOrDefaultAsync(cancellationToken);
            return result;
        }

        public async Task<Dictionary<string, bool>> CheckEmailsInBulk(List<string> emails)
        {
            // Fetch all matching emails in bulk from the database
            var existingEmails = await context.Users
                .Where(u => emails.Contains(u.Email))
                .Select(u => u.Email)
                .ToListAsync();

            // Create a dictionary to mark whether each email is found
            return emails.ToDictionary(email => email, email => existingEmails.Contains(email));
        }

        public async Task<(IEnumerable<UserListDto> Users, int Count)> GetUsersAsync(UserSearchInput baseListingInput, CancellationToken cancellationToken)
        {
            var sortExpression = User.SortBy(baseListingInput.Sorting);
            var query = context.Users.AsNoTracking()
                .WhereIf(
                    !string.IsNullOrWhiteSpace(baseListingInput.SearchString), user =>
                    user.FirstName.Contains(baseListingInput.SearchString, StringComparison.OrdinalIgnoreCase) ||
                    user.Email.Contains(baseListingInput.SearchString, StringComparison.OrdinalIgnoreCase)
                )
               .WhereIf(
                    baseListingInput.LockStatus != LockStatus.All, user =>
                    (baseListingInput.LockStatus == LockStatus.locked && user.IsLocked) ||
                    (baseListingInput.LockStatus == LockStatus.unlocked && !user.IsLocked)
            );

            // Get the total count before pagination
            int count = query.Count();

            // Apply sorting based on the direction
            if (sortExpression.IsDescending)
            {
                query = query.AsQueryable().OrderByDescending(sortExpression.Expression);
            }
            else
            {
                query = query.AsQueryable().OrderBy(sortExpression.Expression);
            }

            var Users = query.Select(user => user.ToUserListDto());

            return (Users, count);
        }

        public async Task<User?> Create(User request)
        {
            var res = await context.Users.AddAsync(request);
            return res.Entity;
        }

        public async Task<UserDetailsDTO?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await context.Users.Where(a => a.Id == id).FirstOrDefaultAsync(cancellationToken);
            return UserMapping.ToUserDetails(result);
        }
        public async Task<User?> GetUserByMailAsync(String Email, CancellationToken cancellationToken)
        {
            return await context.Users.Where(a => a.Email == Email).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User?> FindUserByIdAsync(Guid id)
        {
            return await context.Users.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<bool> Update(User request)
        {
            try
            {
                context.Users.Update(request);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsFound(string Email, Guid Id)
        {
            var res = context.Users.Any(e => e.Email.ToLower() == Email.ToLower() && e.Id != Id);
            return res;
        }
        public async Task<User?> GetUserProfileAsync(Guid id, CancellationToken cancellationToken)
        {
            return await context.Users.Where(u => u.Id == id).FirstOrDefaultAsync(cancellationToken);
        }
        

    }

}
