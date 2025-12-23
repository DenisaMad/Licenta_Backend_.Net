using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DataAbstraction.Security
{
    public interface IHashingSettings
    {
        public string SecretKeyBase64 { get; set; }
        public string CodeVersion { get; set; }
        public int VerificationCodeTtlSeconds { get; set; }
    }
}
