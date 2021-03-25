using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TGS.FileService
{
    public interface IFileManager
    {
        Task<FileMeta> SaveAsync(string filePath, string contentType, long contentLength, Stream fileStream);

        IAsyncEnumerable<string> ListPathAsync(string prefix);

        Task DeleteFilesAsync(IEnumerable<FileMeta> files);

        IAsyncEnumerable<FileMeta> FindExpiredFilesAsync(string prefix, DateTimeOffset expireTime);
    }
}
