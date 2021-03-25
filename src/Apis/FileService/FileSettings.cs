using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace TGS.Apis.FileService
{
    public class FileSettings
    {
        public string Path { get; set; }
        public int FileSizeLimit { get; set; }
    }
}