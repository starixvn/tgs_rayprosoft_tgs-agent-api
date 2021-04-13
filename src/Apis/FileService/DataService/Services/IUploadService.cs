using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Nats.Model;

namespace TGS.Apis.FileService.DataService.Services
{
    public interface IUploadService
    {
        Task<string> UploadFileS3Async(IFormFile formFile);

        Task AddStationLetterFileAsync(ObjectFileEventModel model);

        ValueTask<string[]> GetObjectsAsync(string prefix = null);
    }
}