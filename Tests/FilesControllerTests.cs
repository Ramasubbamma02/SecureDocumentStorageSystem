using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureDocumentStorageSystem.Controllers;
using System.Security.Claims;
using System.Text;
using Xunit;
namespace SecureDocumentStorageSystem.Tests
{
    public class FilesControllerTests
    {
        private FilesController GetController(SecureDocumentContext context, int userId = 1)
        {
            var controller = new FilesController(context);

            // Simulate authenticated user with Claims
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            return controller;
        }

        private SecureDocumentContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SecureDocumentContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB
                .Options;
            return new SecureDocumentContext(options);
        }

        [Fact]
        public async Task Upload_ValidFile_ReturnsOk()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = GetController(context);

            var content = "Hello World";
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(content)), 0, content.Length, "Data", "test.txt")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            // Act
            var result = await controller.Upload(file);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Uploaded", okResult.Value.ToString());
        }

        [Fact]
        public async Task Upload_EmptyFile_ReturnsBadRequest()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = GetController(context);

            var file = new FormFile(new MemoryStream(), 0, 0, "Data", "empty.txt");

            // Act
            var result = await controller.Upload(file);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("File is empty", badResult.Value);
        }

        [Fact]
        public async Task Download_ExistingFile_ReturnsFile()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = GetController(context);

            context.Documents.Add(new Document
            {
                Name = "file.txt",
                Content = Encoding.UTF8.GetBytes("abc"),
                ContentType = "text/plain",
                UploadDate = DateTime.UtcNow,
                Version = 0,
                UserId = 1
            });
            context.SaveChanges();

            // Act
            var result = controller.Download("file.txt");

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("text/plain", fileResult.ContentType);
            Assert.Equal("file.txt", fileResult.FileDownloadName);
        }

        [Fact]
        public void Download_FileNotFound_ReturnsNotFound()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = GetController(context);

            // Act
            var result = controller.Download("nonexistent.txt");

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("File not found", notFound.Value);
        }
    }

}
