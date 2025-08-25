using Backend.DataAbstraction.Security;
using Backend.Domain.User;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services.Security
{
    public class AuthtentificationService: IAuthtentificationServices
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
                    new Claim("name", name),
                    new Claim("role", GetDescription(role)),
                    new Claim("tokenType", "Access" )
                },
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            //cheia si creds trebuie generat doar o data , facem un consoleapp unde rulam linia 41 si 42 si afisam pe ecran key si creds le copiem valorile si inlocuim cu valorile din key si creds
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