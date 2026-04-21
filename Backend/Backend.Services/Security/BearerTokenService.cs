using Backend.DataAbstraction.BearerTokens;
using Backend.DataAbstraction.Security;
using Backend.Domain.User;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services.Security
{
    public class BearerTokenService : IBearerTokenService
    {
        private readonly IAuthentificationService authentificationService;
        private readonly IAuthTokensService authTokensService;
        private readonly IMongoDataBase mongoDataBase;

        public BearerTokenService(
            IAuthentificationService authentificationService,
            IAuthTokensService authTokensService,
            IMongoDataBase mongoDataBase)
        {
            this.authentificationService = authentificationService;
            this.authTokensService = authTokensService;
            this.mongoDataBase = mongoDataBase;
        }

        public async Task<(string AccessToken, string RefreshToken)> IssueNewTokensAsync(User user)
        {

            await BlackList(user);


            var (newAccessToken, newRefreshToken) = GenerateTokens(user);


            await UpdateTokens(user, newAccessToken, newRefreshToken);


            return (newAccessToken, newRefreshToken);
        }

        public (string, string) GenerateTokens(Domain.User.User userFromDB)
        {
            string accessToken = this.authentificationService.GenerateAccessToken(userFromDB.Email, userFromDB.Name, userFromDB.Role, userFromDB.Id);
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
            if (!string.IsNullOrEmpty(userFromDB.RefreshToken))
            {
                await this.authTokensService.BlackListToken(userFromDB.RefreshToken, TokenType.REFRESH_TOKEN, userFromDB.Id);
            }
            if (!string.IsNullOrEmpty(userFromDB.AccessToken))
            {
                await this.authTokensService.BlackListToken(userFromDB.AccessToken, TokenType.ACCESS_TOKEN, userFromDB.Id);
            }
        }
    }
}
