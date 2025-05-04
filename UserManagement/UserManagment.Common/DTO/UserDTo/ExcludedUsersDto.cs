using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagment.Common.DTO.UserDTo
{
    public class ExcludedUsersDto
    {
        public DateOnly DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Groups { get; set; }
        public string Exception { get; set; }
    }
}