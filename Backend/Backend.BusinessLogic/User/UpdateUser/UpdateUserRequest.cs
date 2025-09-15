using MediatR;

namespace Backend.BusinessLogic.User.UpdateUser
{
  public class UpdateUserRequest : IRequest<UpdateUserResponse>
  {
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Id { get; set; } = string.Empty;
  }
}
