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
        private readonly IBearerTokenService bearerTokenService;
        public LoginUserHandler(IMongoDataBase mongoDataBase, IAuthentificationService authentificationService, IHashingService hashingServices, IAuthTokensService authTokensService, IBearerTokenService bearerTokenService)
        {
            this.mongoDataBase = mongoDataBase;
            this.authentificationService = authentificationService;
            this.hashingServices = hashingServices;
            this.authTokensService = authTokensService;
            this.bearerTokenService = bearerTokenService;
        }
        public async Task<LoginUserResponse> Handle(LoginUserRequest request, CancellationToken cancellationToken)
        {
            var userFromDB = await this.FetchUserByEmail(request.Email);

            if (userFromDB == null)
            {
                return new LoginUserResponse();
            }
            if (userFromDB.ActiveAccount == false)
            {
                return new LoginUserResponse();

            }
            bool isPasswordValid = this.hashingServices.VerifyPassword(request.Password, userFromDB.Salt, userFromDB.Password);
            if (!isPasswordValid)
            {
                return new LoginUserResponse();
            }

            var response = await this.bearerTokenService.IssueNewTokensAsync(userFromDB);

            return new LoginUserResponse
            {
                AccessToken = response.Item1,
                RefreshToken = response.Item2
            };

        }
        private async Task<Domain.User.User> FetchUserByEmail(string email)
        {
            var collection = mongoDataBase.GetCollection<Domain.User.User>();
            var filter = Builders<Domain.User.User>.Filter.Eq(x => x.Email, email);
            var userFromDB = await collection.Find(filter).FirstOrDefaultAsync();
            return userFromDB;
        }

    }
}
