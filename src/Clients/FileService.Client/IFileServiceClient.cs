using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TGS.Apis.FileService.Clients
{
    public interface IFileServiceClient
    {
        ValueTask<FileView> UploadFileAsync(string path, MediaTypeHeaderValue contentType, Stream fileStream,
            bool isPublic = false);

        ValueTask<FileView> FindFileByObjectIdAsync(string objectId);

        ValueTask<Stream> OpenFileReaderAsync(string path);
        Task UpdateStationLetterFileAsync(string uploadType, string id, UpdateFilesRequest request);
    }
}