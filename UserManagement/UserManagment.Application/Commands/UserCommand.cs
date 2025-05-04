using MediatR;
using UserManagment.Common.DTO.UserDTo;

namespace UserManagment.Application.Commands
{
    public class UpdateUserCommand : IRequest<bool>
    {
        public UserForUpdateDTO userDto { get; set; }
    }
}