using Backend.BusinessLogic.ProcessMedicineImage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class PhotoController : ControllerBase
    {
        private readonly IMediator mediator;
        public PhotoController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("photo")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadPhoto([FromForm] ProcessMedicineImageRequest request, CancellationToken cancellationToken)
        {
            var response = await this.mediator.Send(request, cancellationToken);
            return this.Ok(response);
        }

    }
}
