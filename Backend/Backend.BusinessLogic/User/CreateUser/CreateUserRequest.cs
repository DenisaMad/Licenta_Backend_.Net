using Backend.Domain.User;
using MediatR;

namespace Backend.BusinessLogic.User.CreateUser
{
  public class CreateUserRequest : IRequest<CreateUserResponse>
  {
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

  }
}
