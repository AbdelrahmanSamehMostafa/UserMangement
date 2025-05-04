using System.Data;
using MediatR;
using UserManagement.Common.Helpers;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.Commands;
using UserManagment.Application.DTOMapping;
using UserManagment.Application.SystemConfiguration;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.Users
{
    public class UserUpdateHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public UserUpdateHandler(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {

            var rowFound = await _unitOfWork.Authentication.FindUserByIdAsync(request.userDto.Id);
            if (rowFound == null)
            {
                throw new CustomException(ErrorResponseMessage.User_NotFound);
            }

            // Validate the email domain
            var domains = await _mediator.Send(new DomainFormat(), cancellationToken);
            var validDomains = domains.Split(";", StringSplitOptions.RemoveEmptyEntries)
                .Select(d => d.Trim()).ToList(); //all valid domains

            if (!validDomains.Contains(request.userDto.Email.Split('@').Last()))
            {
                throw new CustomException(ErrorResponseMessage.User_MailDomain);
            }

            // validate user already exists
            bool isRepeated = await _unitOfWork.Authentication.IsFound(request.userDto.Email, request.userDto.Id);
            if (isRepeated)
            {
                throw new CustomException(ErrorResponseMessage.isRepeated);
            }
            else
            {
                // Fetch existing group IDs
                var existingGroupIds = await _unitOfWork.GroupUser.GetUserGroupIdsAsync(request.userDto.Id, cancellationToken);
                if (!request.userDto.GroupIds.Any())
                {
                    throw new CustomException(ErrorResponseMessage.Group_NotFound);
                }

                // Call the generic function to update the groups
                await GenericFunctions.UpdateCollectionAsync(
                    existingGroupIds,
                    request.userDto.GroupIds,
                    _unitOfWork.GroupUser.addGroupsUserAsync,
                    _unitOfWork.GroupUser.RemoveUserGroupsAsync,
                    request.userDto.Id,
                    cancellationToken);

                var userResultDto = UserMapping.UserForUpdate(request.userDto, rowFound);
                userResultDto.UpdatedDate = DateTime.Now; // To be optimizd 
                var result = await _unitOfWork.Authentication.Update(userResultDto);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return result;
            }
        }
    }
}