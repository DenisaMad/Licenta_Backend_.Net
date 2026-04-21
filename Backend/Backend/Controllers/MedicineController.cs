using Backend.BusinessLogic.CreateMedicine;
using Backend.DataAbstraction.Security;
using Backend.Domain.MedicineDomain;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.WebRequestMethods;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class MedicineController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MedicineController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{name}")]
        public async Task<IActionResult> RegisterMedicine(string name)
        {
            var request = new CreateMedicineRequest { Name = name };
            var response = await _mediator.Send(request);

            if (!response.Success)
                return BadRequest("Could not register medicine.");

            return Ok(response);
        }
    }
}
