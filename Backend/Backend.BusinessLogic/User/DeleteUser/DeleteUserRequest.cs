using MediatR;

namespace Backend.BusinessLogic.User.DeleteUser
{
  public class DeleteUserRequest : IRequest<DeleteUserResponse>
  {
    public string Id { get; set; } = string.Empty;
  }
}
