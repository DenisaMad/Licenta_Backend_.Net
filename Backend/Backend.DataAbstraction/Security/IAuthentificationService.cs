namespace Backend.DataAbstraction.Security
{
  public interface IAuthentificationService
  {
    public string GenerateAccessToken(string email, string name, Enum role);

    public string GenerateRefreshToken();

    public string GetDescription(Enum value);

    public Task<bool> CheckActive(string userEmail);


  }
}
