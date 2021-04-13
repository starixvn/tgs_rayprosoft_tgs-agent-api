using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.FileService.Controllers.Messages;
using TGS.FileService;

namespace TGS.Apis.FileService.Controllers
{
    [Route("api/fs")]
    [ApiController]
    public class FileManagementController : ControllerBase
    {
        private readonly IFileManager _fileManager;

        public FileManagementController(IFileManager fileManager)
        {
            _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
        }

        [HttpGet("{*path}")]
        public async Task<IActionResult> GetFileAsync(
            string path)
        {
            var meta = await _fileManager.ListPathAsync(path)
                .FirstAsync()
                .ConfigureAwait(false);

            var fileStream = await _fileManager.OpenFileStreamAsync(meta).ConfigureAwait(false);

            return File(fileStream, meta.ContentType);
        }

        [HttpPost("{*path}")]
        public async Task<FileViewModel> UploadFileAsync(
            string path,
            [FromForm(Name = "file")] IFormFile file)
        {
            var meta = await _fileManager
                .SaveAsync(path, file.ContentType, file.Length, file.OpenReadStream())
                .ConfigureAwait(false);

            return new FileViewModel
            {
                Id = meta.Id,
                FileName = meta.FileName,
                ContentType = meta.ContentType,
                FileLength = meta.FileLength,
                CreatedOn = meta.CreatedOn,
                ExternalUrl = meta.ExternalUrl
            };
        }
    }
}
