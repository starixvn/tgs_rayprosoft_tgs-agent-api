using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.FileService.Controllers.Messages;
using TGS.Core.Infrastructures.Aws;
using TGS.FileService;

namespace TGS.Apis.FileService.Controllers
{
    [Route("api/fs/public")]
    [ApiController]
    public class PublicFileManagementController : ControllerBase
    {
        private readonly IFileManager<IPublicAmazonS3> _fileManager;

        public PublicFileManagementController(IFileManager<IPublicAmazonS3> fileManager)
        {
            _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
        }

        [HttpPost("{*path}")]
        public async Task<FileViewModel> UploadFileAsync(
            string path,
            [FromForm(Name = "file")] IFormFile file)
        {
            var meta = await _fileManager
                .SaveAsync(path, file.ContentType, file.Length, file.OpenReadStream())
                .ConfigureAwait(false);

            return new()
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