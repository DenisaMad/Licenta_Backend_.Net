using Backend.DataAbstraction.Security;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Backend.Services.Security
{
  public class HashingService: IHashingService
    {
        private readonly IHashingSettings settings;
        public  HashingService(IHashingSettings settings)
        {
            this.settings = settings;
        }
        public string HashPassword(string password, string salt)
        {

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hashed;

        }

        public bool VerifyPassword(string password, string salt, string hashed)
        {

            string hashed2 = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
            
            return hashed2 == hashed;


        }
    
        public string GenerateSalt()
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
            return Convert.ToBase64String(salt);
        }


        public string GenerateRandomCode()
        {
            byte[] randomNumber = new byte[6];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            var result = string.Empty;
            foreach (var b in randomNumber)
            {
                result += (b % 10).ToString();
            }

            return result;
        }
        public bool ValidCode(string code) 
        {
            if (string.IsNullOrWhiteSpace(code)) return false;

            var parts = code.Split('.', 4);
            if (parts.Length != 4) return false;

            var ver = parts[0];
            var expStr = parts[1];
            var rndB64 = parts[2];
            var sigProvidedB64 = parts[3];

            if (!string.Equals(ver, settings.CodeVersion, StringComparison.Ordinal)) return false;
            if (!long.TryParse(expStr, out long exp)) return false;

            // Recalculează semnătura și compară în constant-time
            string data = $"{ver}.{expStr}.{rndB64}";
            var sigExpected = ComputeHmacSha256(Encoding.UTF8.GetBytes(data));
            if (!FixedTimeEquals(FromBase64Url(sigProvidedB64), sigExpected)) return false;

            // Verifică expirarea
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (now > exp) return false;

            return true;
        }
        private byte[] ComputeHmacSha256(byte[] data)
        {
            var key = Convert.FromBase64String(settings.SecretKeyBase64);
            using var hmac = new System.Security.Cryptography.HMACSHA256(key);
            return hmac.ComputeHash(data);
        }

        private  bool FixedTimeEquals(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
        {
            if (a.Length != b.Length) return false;
            return System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(a, b);
        }

        private  string Base64UrlEncode(ReadOnlySpan<byte> data)
        {
            var s = Convert.ToBase64String(data);
            return s.Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }

        private  byte[] FromBase64Url(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/');
            switch (s.Length % 4)
            {
                case 2: s += "=="; break;
                case 3: s += "="; break;
            }
            return Convert.FromBase64String(s);
        }
    }
}
