using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TGS.FileService
{
    public interface IFileMetaRepository
    {
        ValueTask<string> AddFileMetaAsync(FileMeta meta);

        IAsyncEnumerable<FileMeta> ListExpiredFileMetasAsync(string prefix, DateTimeOffset expireTime);

        IAsyncEnumerable<FileMeta> ListFileMetasAsync(string prefix);

        ValueTask DeleteFileMetaByIdsAsync(IEnumerable<string> fileIds);
    }
}
