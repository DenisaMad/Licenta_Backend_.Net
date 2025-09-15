using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.BusinessLogic.User.CreateUser
{
    public class CreateUserValidator:AbstractValidator<CreateUserRequest>
    {
        public CreateUserValidator() { 
            this.RuleFor(request=>request.Email).NotEmpty().WithMessage("Email cannot be empty!").Must(x=>x.Contains("@yahoo.com")).WithMessage("Invalid email format");
            this.RuleFor(request => request.Password).NotEmpty().WithMessage("Password cannot be empty!").Must(x => x.Length>6).WithMessage("Password shoud be grater than 6 characters");
        }
    }
}
