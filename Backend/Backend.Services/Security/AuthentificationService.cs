using Backend.DataAbstraction.Security;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace Backend.Services.Security
{
  public class AuthentificationService: IAuthentificationService
    {
        public string GenerateAccessToken(string email, string name, Enum role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("bahdoiajfncajnfhoijnaoifanif131jdangsfdsbahdoiajfncajnfhoijnaoifanif131jdangsfdsbahdoiajfncajnfhoijnaoifanif131jdangsfds"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "your_issuer",
                audience: "your_audience",
                claims: new List<Claim>()
                { 
                    new Claim("email", email),
                    new Claim("name", name ?? string.Empty),
                    new Claim("role", GetDescription(role)),
                    new Claim("tokenType", "Access" )
                },
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("bahdoiajfncajnfhoijnaoifanif131jdangsfdsbahdoiajfncajnfhoijnaoifanif131jdangsfdsbahdoiajfncajnfhoijnaoifanif131jdangsfds"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "your_issuer",
                audience: "your_audience",
                claims: new List<Claim>(),
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GetDescription(Enum value)
        {
            return value
                .GetType()
                .GetField(value.ToString())?
                .GetCustomAttribute<DescriptionAttribute>()?
                .Description ?? value.ToString();
        }
    }
}