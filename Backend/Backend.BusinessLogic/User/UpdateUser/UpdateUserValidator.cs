using FluentValidation;

namespace Backend.BusinessLogic.User.UpdateUser
{
  public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
  {
    public UpdateUserValidator() {
      this.RuleFor(request => request.Password).MinimumLength(5).WithMessage("Password should be grater than 10");
      this.RuleFor(request => request.Email).Must(em => em.Contains("@")).WithMessage("Invalid email format");
      this.RuleFor(request => request.Name).MinimumLength(2).WithMessage("Name should be greater or equal than 2");
      this.RuleFor(request => request.Id).NotEmpty().WithMessage("Id required!");
    }
  }
}
