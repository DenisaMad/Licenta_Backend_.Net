using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    public class UploadPhotoRequest
    {
        public IFormFile File { get; set; } = default!;
    }

    [ApiController]
    [Route("[controller]")]
    public class PhotoController : ControllerBase
    {
        [HttpPost("photo")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadPhoto([FromForm] UploadPhotoRequest request)
        {
            var file = request.File;
            Console.WriteLine($"FileName: {file.FileName}");
            Console.WriteLine($"ContentType: {file.ContentType}");
            Console.WriteLine($"Length: {file.Length} bytes");

            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var bytes = ms.ToArray();


            return Ok(new
            {
                message = "Upload OK",
                file.FileName,
                file.ContentType,
                file.Length
            });
        }

    }
}
