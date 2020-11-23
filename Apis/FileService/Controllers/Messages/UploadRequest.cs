using Microsoft.AspNetCore.Http;

namespace TGS.Apis.FileService.Controllers.Messages
{
    public class UploadRequest
    {
        public IFormFile File { get; set; }
    }
}