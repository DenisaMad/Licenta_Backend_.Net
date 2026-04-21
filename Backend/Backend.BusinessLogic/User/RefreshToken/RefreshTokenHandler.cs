using Backend.DataAbstraction.Security;
using Backend.DataAbstraction.Database;
using MediatR;
using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;
using Backend.Domain.User;
using System.Linq;
using Backend.DataAbstraction.BearerTokens;

namespace Backend.BusinessLogic.User.RefreshToken
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenRequest, RefreshTokenResponse>
    {
        private readonly IMongoDataBase mongoDataBase;
        private readonly IBearerTokenService bearerTokenService;
        private readonly IAuthTokensService authTokensService;

        public RefreshTokenHandler(IMongoDataBase mongoDataBase, IBearerTokenService bearerTokenService, IAuthTokensService authTokensService)
        {
            this.mongoDataBase = mongoDataBase;
            this.bearerTokenService = bearerTokenService;
            this.authTokensService = authTokensService;
        }

        public async Task<RefreshTokenResponse> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return new RefreshTokenResponse { Success = false, ErrorMessage = "Refresh token is missing." };
            }

            var collection = mongoDataBase.GetCollection<Domain.User.User>();
            var filter = Builders<Domain.User.User>.Filter.Eq(x => x.RefreshToken, request.RefreshToken);
            var userFromDb = await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);

            if (userFromDb == null)
            {
                return new RefreshTokenResponse { Success = false, ErrorMessage = "Invalid refresh token." };
            }

            // Check if it's blacklisted
            bool isBlacklisted = await authTokensService.CheckBlacklistedToken(request.RefreshToken, TokenType.REFRESH_TOKEN, userFromDb.Id);
            if (isBlacklisted)
            {
                return new RefreshTokenResponse { Success = false, ErrorMessage = "Refresh token is blacklisted." };
            }

            // Generate new ones, blacklist old ones, and save to DB
            var response = await bearerTokenService.IssueNewTokensAsync(userFromDb);

            return new RefreshTokenResponse
            {
                Success = true,
                AccessToken = response.AccessToken,
                RefreshToken = response.RefreshToken
            };
        }
    }
}
