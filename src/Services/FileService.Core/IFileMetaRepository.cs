using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TGS.FileService
{
    public interface IFileMetaRepository
    {
        ValueTask<string> AddFileMetaAsync(FileMeta meta);

        IAsyncEnumerable<FileMeta> ListExpiredFileMetasAsync(string prefix, DateTimeOffset expireTime);

        ValueTask DeleteFileMetaByIdsAsync(IEnumerable<string> fileIds);
    }
}
