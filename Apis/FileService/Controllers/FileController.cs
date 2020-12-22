using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.FileService.Controllers.Messages;
using TGS.Apis.FileService.DataService.Services;

namespace TGS.Apis.FileService.Controllers
{
    [ApiController]
    [Route("fs/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IUploadService _uploadService;

        public FileController(IUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        [HttpPost("uploadFileS3")]
        public async Task<UploadResponse> UploadFileS3Async([FromForm] UploadRequest request)
        {
            var url = await _uploadService.UploadFileS3Async(request.File);
            return new UploadResponse
            {
                Valid = true,
                Location = url
            };
        }

        [HttpGet("{bucket}")]
        public async Task<GetFileNamesResponse> GetFileNamesFromS3Async(string bucket)
        {
            var keys = await _uploadService.GetObjectsAsync(bucket);
            return new GetFileNamesResponse
            {
                Valid = true,
                Names = keys
            };
        }
    }
}