using MediatR;

namespace Backend.BusinessLogic.User.RefreshToken
{
    public class RefreshTokenRequest : IRequest<RefreshTokenResponse>
    {
        public string RefreshToken { get; set; }
    }
}
