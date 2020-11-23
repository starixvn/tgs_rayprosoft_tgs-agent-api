using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.FileService.Controllers.Messages;
using TGS.Apis.FileService.DataService.Services;

namespace TGS.Apis.FileService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IUploadService _uploadService;

        public FileController(IUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        [HttpPost("upload")]
        public async Task<UploadResponse> UploadFileAsync([FromForm] UploadRequest request)
        {
            var url = await _uploadService.UploadFileAsync(request.File);
            return new UploadResponse
            {
                Valid = true,
                Url = url
            };
        }

        [HttpPost("uploadFileS3")]
        public async Task<UploadResponse> UploadFileS3Async([FromForm] UploadRequest request)
        {
            var url = await _uploadService.UploadFileS3Async(request.File);
            return new UploadResponse
            {
                Valid = true,
                Url = url
            };
        }
    }
}