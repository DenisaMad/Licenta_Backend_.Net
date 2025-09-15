using FluentValidation;

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
