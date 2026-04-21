using Backend.DataAbstraction.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class CryptoController : ControllerBase
    {
        private readonly IRsaKeyProvider _rsaKeyProvider;

        public CryptoController(IRsaKeyProvider rsaKeyProvider)
        {
            _rsaKeyProvider = rsaKeyProvider;
        }

        [HttpGet("PublicKey")]
        public IActionResult GetPublicKey()
        {
            return Ok(new { PublicKey = _rsaKeyProvider.GetPublicKeyBase64() });
        }
    }
}
