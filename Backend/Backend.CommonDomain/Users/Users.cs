using Backend.Domain.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.CommonDomain.Users
{
    public class LoginUserRequest {

        public string Password { get; set; }
        public string Email { get; set; }
    }
}
