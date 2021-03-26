using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("{*path}")]
        public Task<FileMeta> UploadFile(string path, IFormFile file)
        {
            return _fileManager.SaveAsync(path, file.ContentType, file.Length, file.OpenReadStream());
        }
    }
}
