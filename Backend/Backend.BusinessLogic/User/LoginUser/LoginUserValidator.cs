using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.BusinessLogic.User.LoginUser
{
    public class LoginUserValidator : AbstractValidator<LoginUserRequest>
    {
        public LoginUserValidator(){
            this.RuleFor(request => request.Email).NotEmpty().WithMessage("Email cannot be empty!").EmailAddress().WithMessage("Invalid email format!");
            this.RuleFor(request => request.Password).NotEmpty().WithMessage("Password cannot be empty!").MinimumLength(6).WithMessage("Password should be greater than 6 characters!");
        }
    }
}
