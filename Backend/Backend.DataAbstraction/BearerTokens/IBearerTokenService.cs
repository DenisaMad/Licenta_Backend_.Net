using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Backend.DataAbstraction.BearerTokens
{
    public interface IBearerTokenService
    {
        public (string ,string ) GenerateTokens(Domain.User.User userFromDB);
        Task<(string AccessToken, string RefreshToken)> IssueNewTokensAsync(Domain.User.User user);
    }
}
