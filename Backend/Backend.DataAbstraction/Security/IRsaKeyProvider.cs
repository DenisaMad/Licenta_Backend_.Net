using System.Security.Cryptography;

namespace Backend.DataAbstraction.Security
{
    public interface IRsaKeyProvider
    {
        RSA GetRsa();
        string GetPublicKeyBase64();
    }
}
