using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.FileService;

namespace TGS.Apis.FileService.DataService.Services
{
    public interface IUploadService
    {
        Task<FileMeta> UploadFileS3Async(string prefix, IFormFile formFile);

        ValueTask<string[]> GetObjectsAsync(string prefix = null);
        Task CleanFilesAsync(string prefix, string[] names);
    }
}