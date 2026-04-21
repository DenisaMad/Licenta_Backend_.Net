using Backend.DataAbstraction.Security;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography;

namespace Backend.Services.Security
{
    public class RsaKeyProvider : IRsaKeyProvider
    {
        private readonly RSA _rsa;
        private readonly string _publicKeyBase64;

        public RsaKeyProvider(IConfiguration configuration)
        {
            _rsa = RSA.Create(2048);

            var privKeyB64 = configuration["EncryptionSettings:RsaPrivateKeyBase64"];
            var pubKeyB64 = configuration["EncryptionSettings:RsaPublicKeyBase64"];

            if (!string.IsNullOrEmpty(privKeyB64))
            {
                _rsa.ImportRSAPrivateKey(Convert.FromBase64String(privKeyB64), out _);
                _publicKeyBase64 = pubKeyB64 ?? Convert.ToBase64String(_rsa.ExportSubjectPublicKeyInfo());
            }
            else
            {
                _publicKeyBase64 = Convert.ToBase64String(_rsa.ExportSubjectPublicKeyInfo());
            }
        }

        public RSA GetRsa() => _rsa;
        public string GetPublicKeyBase64() => _publicKeyBase64;
    }
}
