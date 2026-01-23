using MediatR;
using Microsoft.AspNetCore.Http;

namespace Backend.BusinessLogic.ProcessMedicineImage
{
  public class ProcessMedicineImageRequest : IRequest<ProcessMedicineImageResponse>
  {
    public IFormFile File { get; set; } = default!;

    public string UserId { get; set; } = default!;
  }
}
