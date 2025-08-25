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
            this.RuleFor(request => request.PhoneNumber).NotEmpty().WithMessage("Phone number cannot be empty!").Must(x => x.Length==24).WithMessage("Phone number shoud be completed");
            this.RuleFor(request => request.Age).NotEmpty().WithMessage("Age cannot be empty!").Must(x => x > 14).WithMessage("Age must be greater than 14!");
            this.RuleFor(request => request.Name).NotEmpty().WithMessage("Name cannot be empty!").Must(x => x.Length > 6).WithMessage("Name shoud be completed");

        }
    }
}
