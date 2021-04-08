using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TGS.Apis.FileService.Clients
{
    public interface IFileServiceClient
    {
        ValueTask<FileView> UploadFileAsync(string path, MediaTypeHeaderValue contentType, Stream fileStream);
    }
}