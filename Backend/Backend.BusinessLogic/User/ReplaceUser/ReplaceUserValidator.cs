using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.BusinessLogic.User.ReplaceUser
{
    public class ReplaceUserValidator : AbstractValidator<ReplaceUserRequest>
    {
        public ReplaceUserValidator()
        {
            this.RuleFor(request => request.Email).NotEmpty().WithMessage("Email cannot be empty!").EmailAddress().WithMessage("Invalid email format!");

            this.RuleFor(request => request.Password).NotEmpty().WithMessage("Password cannot be empty!").MinimumLength(6).WithMessage("Password should be greater than 6 characters!");

            this.RuleFor(request => request.PhoneNumber).NotEmpty().WithMessage("Phone number cannot be empty!").Length(10).WithMessage("Phone number must be exactly 10 digits!");

            this.RuleFor(request => request.Age).NotEmpty().WithMessage("Age cannot be empty!").GreaterThan(14).WithMessage("Age must be greater than 14!");

            this.RuleFor(request => request.Name).NotEmpty().WithMessage("Name cannot be empty!").MinimumLength(3).WithMessage("Name should be at least 3 characters long!");
        }
    }
}
