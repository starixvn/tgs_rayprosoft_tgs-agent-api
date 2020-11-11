using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace TGS.Core.Infrastructures.File
{
    public interface IFileService
    {
        public Task<string> UploadFileAsync(IFormFile formFile);
    }

    public class FileService : IFileService
    {
        private readonly IHostEnvironment _env;
        private readonly IOptions<FileSetting> _fileSetting;
        private readonly IWebHostEnvironment _hostEnvironment;

        public FileService(IHostEnvironment env, IWebHostEnvironment hostEnvironment, IOptions<FileSetting> fileSetting)
        {
            _env = env;
            _fileSetting = fileSetting;
            _hostEnvironment = hostEnvironment;
        }

        async Task<string> IFileService.UploadFileAsync(IFormFile formFile)
        {
            Directory.CreateDirectory(_fileSetting.Value.Path);
            var randomFileName = Path.GetRandomFileName() + Path.GetExtension(formFile.FileName);
            var path = Path.Join(_fileSetting.Value.Path, "/", randomFileName);
            await using var stream = System.IO.File.Create(path);
            await formFile.CopyToAsync(stream);

            return path;
        }
    }
}