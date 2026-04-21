using Backend.BusinessLogic.CreateMedicine;
using Backend.BusinessLogic.MedicineDTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class SaveExtractedMedicineController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SaveExtractedMedicineController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost()]
        public async Task<IActionResult> RegisterMedicine(MedicineRequest request)
        {
            var response = await _mediator.Send(request);

            if (!response.Success)
                return BadRequest("Could not register medicine.");

            return Ok(response);
        }
    }
}

