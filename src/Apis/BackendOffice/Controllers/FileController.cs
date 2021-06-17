using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.FileService.Clients;
using TGS.Core.Models;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/fs")]
    [ApiController]
    public class FileController : ControllerBase
    {
        /// <summary>
        ///     Gets the file.
        /// </summary>
        /// <param name="fileServiceClient">The file service client.</param>
        /// <param name="objectId">The object identifier.</param>
        /// <returns></returns>
        [HttpGet("{objectId}")]
        public async Task<IActionResult> GetFileAsync(
            [FromServices] IFileServiceClient fileServiceClient,
            string objectId)
        {
            var fileView = await fileServiceClient.FindFileByObjectIdAsync(objectId).ConfigureAwait(false);

            return File(
                await fileServiceClient
                    .OpenFileReaderAsync(fileView.FileName)
                    .ConfigureAwait(false),
                fileView.ContentType);
        }

        [HttpPost("upload/{prefix}")]
        public async Task<UploadResponse> UploadFileAsync([FromServices] IFileServiceClient fileServiceClient,
            string prefix,
            [FromForm(Name = "file")] IFormFile file)
        {
            var category = Consts.UploadFile.Types.FirstOrDefault(name => name == prefix);

            var contentType = file.ContentType;
            await using var fileStream = file.OpenReadStream();

            var fileView = await fileServiceClient.UploadPublicFileAsync(category, file.FileName,
                MediaTypeHeaderValue.Parse(contentType), fileStream);
            return new() {Valid = true, Location = fileView.ExternalUrl};
        }
    }
}