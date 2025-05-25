using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SecureDocumentStorageSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly SecureDocumentContext _context;

        public FilesController(SecureDocumentContext context)
        {
            _context = context;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized();

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var existingVersions = _context.Documents
                .Where(d => d.Name == file.FileName && d.UserId == userId)
                .OrderByDescending(d => d.Version)
                .ToList();

            var newVersion = existingVersions.Any() ? existingVersions.First().Version + 1 : 0;

            var document = new Document
            {
                Name = file.FileName,
                Content = memoryStream.ToArray(),
                ContentType = file.ContentType,
                Version = newVersion,
                UploadDate = DateTime.UtcNow,
                UserId = userId
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return Ok(new { message = "File uploaded", version = newVersion });
        }

        [HttpGet("{fileName}")]
        public IActionResult Download(string fileName, [FromQuery] int? version = null)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized();

            var query = _context.Documents
                .Where(d => d.Name == fileName && d.UserId == userId);

            var document = version.HasValue
                ? query.FirstOrDefault(d => d.Version == version.Value)
                : query.OrderByDescending(d => d.Version).FirstOrDefault();

            if (document == null)
                return NotFound("File not found");

            return File(document.Content, document.ContentType, document.Name);
        }
    }


}
