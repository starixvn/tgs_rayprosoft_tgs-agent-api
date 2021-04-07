using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.FileService.Controllers.Messages;
using TGS.FileService;
using Yarp.ReverseProxy.Service;
using Yarp.ReverseProxy.Service.Proxy;
using Microsoft.Extensions.DependencyInjection;
using Yarp.ReverseProxy.Abstractions;

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
            [FromServices] IHttpProxy httpProxy,
            [FromServices] ITransformBuilder transformBuilder,
            string path)
        {
            var meta = await _fileManager.GetFileAsync(path);

            throw new NotImplementedException();
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
