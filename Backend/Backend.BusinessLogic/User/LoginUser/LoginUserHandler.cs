using Backend.DataAbstraction.BearerTokens;
using Backend.DataAbstraction.Security;
using Backend.Domain.User;
using MediatR;
using MongoDB.Driver;

namespace Backend.BusinessLogic.User.LoginUser
{
  public class LoginUserHandler : IRequestHandler<LoginUserRequest, LoginUserResponse>
  {
    private readonly IMongoDataBase mongoDataBase;
    private readonly IHashingService hashingServices;
    private readonly IAuthentificationService authentificationService;
    private readonly IAuthTokensService authTokensService;
    public LoginUserHandler(IMongoDataBase mongoDataBase, IAuthentificationService authentificationService, IHashingService hashingServices, IAuthTokensService authTokensService)
    {
      this.mongoDataBase = mongoDataBase;
      this.authentificationService = authentificationService;
      this.hashingServices = hashingServices;
      this.authTokensService = authTokensService;
    }
    public async Task<LoginUserResponse> Handle(LoginUserRequest request, CancellationToken cancellationToken)
    {
      var userFromDB = await this.FetchUserByEmail(request.Email);

      if (userFromDB == null)
      {
        return new LoginUserResponse();
      }

      bool isPasswordValid = this.hashingServices.VerifyPassword(request.Password, userFromDB.Salt, userFromDB.Password);
      if (!isPasswordValid)
      {
        return new LoginUserResponse();
      }
      var (accessToken, refreshToken) = GenerateTokens(userFromDB);
      await this.BlackList(userFromDB);
      await this.UpdateTokens(userFromDB, accessToken, refreshToken);

      return new LoginUserResponse
      {
        AccessToken = accessToken,
        RefreshToken = refreshToken
      };
    }
    private async Task<Domain.User.User> FetchUserByEmail(string email)
    {
      var collection = mongoDataBase.GetCollection<Domain.User.User>();
      var filter = Builders<Domain.User.User>.Filter.Eq(x => x.Email, email);
      var userFromDB = await collection.Find(filter).FirstOrDefaultAsync();
      return userFromDB;
    }
    private (string access,string refresh) GenerateTokens(Domain.User.User userFromDB)
    {
      string accessToken = this.authentificationService.GenerateAccessToken(userFromDB.Email, userFromDB.Name, userFromDB.Role);
      string refreshToken = this.authentificationService.GenerateRefreshToken();
      return (accessToken, refreshToken);
    }
    private async Task UpdateTokens(Domain.User.User userFromDB, string accessToken, string refreshToken)
    {
      userFromDB.AccessToken = accessToken;
      userFromDB.RefreshToken = refreshToken;

      var updateFilter = Builders<Domain.User.User>.Filter.Eq(x => x.Id, userFromDB.Id);
      await this.mongoDataBase.GetCollection<Domain.User.User>().ReplaceOneAsync(updateFilter, userFromDB);
    }
    private async Task BlackList(Domain.User.User userFromDB)
    {
      string refreshToken = this.authentificationService.GenerateRefreshToken();
      if (string.IsNullOrEmpty(userFromDB.RefreshToken))
      {
        await this.authTokensService.BlackListToken(userFromDB.RefreshToken, TokenType.REFRESH_TOKEN, userFromDB.Id);
      }
      if (string.IsNullOrEmpty(userFromDB.AccessToken))
      {
        await this.authTokensService.BlackListToken(userFromDB.AccessToken, TokenType.ACCESS_TOKEN, userFromDB.Id);
      }
    }
  }
}
