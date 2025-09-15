using MediatR;

namespace Backend.BusinessLogic.User.LoginUser
{
  public class LoginUserRequest : IRequest<LoginUserResponse>
  {
    public string Email { get; set; }
    public string Password { get; set; }
  }
}
