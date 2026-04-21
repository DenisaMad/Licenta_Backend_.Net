namespace Backend.BusinessLogic.User.RefreshToken
{
    public class RefreshTokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public bool Success { get; set; } = true;
        public string ErrorMessage { get; set; }
    }
}
