using Backend.DataAbstraction.Security;
using FluentValidation;

namespace Backend.BusinessLogic.User.LoginUser
{
  public class LoginUserValidator : AbstractValidator<LoginUserRequest>
  {
    public LoginUserValidator(IAuthentificationService authentificationService)
    {
      this.RuleFor(request => request.Email).NotEmpty().WithMessage("Email cannot be empty!").EmailAddress().WithMessage("Invalid email format!");
      this.RuleFor(request => request.Password).NotEmpty().WithMessage("Password cannot be empty!").MinimumLength(6).WithMessage("Password should be greater than 6 characters!");
      this.RuleFor(request => request.Email).MustAsync(async (email,ctx) =>
      {
        return await authentificationService.CheckActive(email);
      }).WithMessage("User inactive");
    }
  }
}
