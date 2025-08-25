using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DataAbstraction.Security
{
    public interface IAuthtentificationServices
    {
        public string GenerateAccessToken(string email, string name, Enum role);

        public string GenerateRefreshToken();

        public string GetDescription(Enum value);

    }
}
