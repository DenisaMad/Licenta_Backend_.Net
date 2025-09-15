using Backend.BusinessLogic.Responses;

namespace Backend.BusinessLogic.User.LoginUser
{
  public class LoginUserResponse : Response
  {
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
  }
}
