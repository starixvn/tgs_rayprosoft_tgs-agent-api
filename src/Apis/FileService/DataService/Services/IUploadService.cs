using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TGS.Apis.FileService.DataService.Services
{
    public interface IUploadService
    {
        Task<string> UploadFileS3Async(string prefix, IFormFile formFile);

        ValueTask<string[]> GetObjectsAsync(string prefix = null);
        Task CleanFilesAsync(string prefix);
    }
}