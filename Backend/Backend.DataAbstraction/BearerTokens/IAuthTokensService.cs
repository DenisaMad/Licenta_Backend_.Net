namespace Backend.DataAbstraction.BearerTokens
{
  public interface IAuthTokensService
  {
    //WILL SAVE THE token IN THE LIST OF TOKENS FOR USER WITH THE ID userId
    public Task BlackListToken(string token, TokenType tokenType, string userId);

    //WILL CHECK IF A TOKEN IS BLACKLISTED OR NOT
    public Task<bool> CheckBlacklistedToken(string token, TokenType tokenType, string userId);
  }
}
