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
    public class AuthTokensService : IAuthTokensService
    {
        private readonly IMongoDataBase _db;

        public AuthTokensService(IMongoDataBase db)
        {
            _db = db;
        }

        public async Task BlackListToken(string token, TokenType tokenType, string userId)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(userId)) return;

            var col = _db.GetCollection<GeneratedToken>();

            var doc = new GeneratedToken
            {
                Token = token,
                Type = tokenType
            };

            await col.InsertOneAsync(doc);
        }

        public async Task<bool> CheckBlacklistedToken(string token, TokenType tokenType, string userId)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(userId)) return false;

            var col = _db.GetCollection<GeneratedToken>();

            var filter = Builders<GeneratedToken>.Filter.And(
              Builders<GeneratedToken>.Filter.Eq(x => x.Type, tokenType),
              Builders<GeneratedToken>.Filter.Eq(x => x.Token, token)
            );

            return await col.Find(filter).AnyAsync();
        }
    }
}
