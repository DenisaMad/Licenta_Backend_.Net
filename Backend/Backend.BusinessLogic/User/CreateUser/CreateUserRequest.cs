using Backend.Domain.User;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.BusinessLogic.User.CreateUser
{
    public class CreateUserRequest:IRequest<CreateUserResponse>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string PhoneNumber { get; set; }
        public int Age { get; set; }
        public EUserRole Role { get; set; }


    }
}
