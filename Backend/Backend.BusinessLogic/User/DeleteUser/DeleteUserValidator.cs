using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.BusinessLogic.User.DeleteUser
{
    public class DeleteUserValidator : AbstractValidator<DeleteUserRequest>
    {
        public DeleteUserValidator()
        {
            this.RuleFor(request => request.Id).NotEmpty().WithMessage("User ID cannot be empty!");
        }
    }
}
