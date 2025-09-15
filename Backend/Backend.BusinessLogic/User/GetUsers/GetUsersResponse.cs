using Backend.BusinessLogic.Responses;

namespace Backend.BusinessLogic.User.GetUsers
{
  public class GetUsersResponse : Response
  {
    public List<Domain.User.User> Users {  get; set; }
  }
}
