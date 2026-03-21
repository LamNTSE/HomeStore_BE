using HomeStore.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HomeStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "10. Upload")]
public class UploadController : ControllerBase
{
    private readonly IFileService _fileService;

    public UploadController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is empty");

        var filePath = await _fileService.UploadImageAsync(file);

        // 🔥 TẠO FULL URL
        var request = HttpContext.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";
        var fullUrl = baseUrl + filePath;

        return Ok(new
        {
            FilePath = fullUrl
        });
    }
}