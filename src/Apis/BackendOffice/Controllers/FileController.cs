using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.FileService.Clients;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/fs")]
    [ApiController]
    public class FileController : ControllerBase
    {
        /// <summary>
        /// Gets the file.
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
    }
}
