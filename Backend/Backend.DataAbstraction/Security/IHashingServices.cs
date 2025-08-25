using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DataAbstraction.Security
{
    public interface IHashingServices
    {
        public string HashPassword(string password, string salt);

        public bool VerifyPassword(string password, string salt, string hashed);

        public string GenerateSalt();
       
    }
}
