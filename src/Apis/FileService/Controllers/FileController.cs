using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.FileService.Controllers.Messages;
using TGS.Apis.FileService.DataService.Services;
using TGS.Core.Models;

namespace TGS.Apis.FileService.Controllers
{
    [ApiController]
    [Route("fs/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IPromotionService _promotionService;
        private readonly IStationLetterService _stationLetterService;
        private readonly IUploadService _uploadService;

        public FileController(IUploadService uploadService, IStationLetterService stationLetterService,
            IPromotionService promotionService)
        {
            _uploadService = uploadService;
            _stationLetterService = stationLetterService;
            _promotionService = promotionService;
        }

        [HttpPost("upload/{prefix}")]
        public async Task<FileViewModel> UploadFileAsync(string prefix, [FromForm(Name = "file")] IFormFile file)
        {
            var meta = await _uploadService.UploadFileS3Async(prefix, file);
            return new()
            {
                Id = meta.Id,
                FileName = meta.FileName,
                ContentType = meta.ContentType,
                FileLength = meta.FileLength,
                CreatedOn = meta.CreatedOn,
                ExternalUrl = meta.ExternalUrl
            };
        }

        [HttpPut("stationLetter/{id}")]
        public async Task<IActionResult> AddStationLetterFileAsync(string id, [FromForm] UpdateFilesRequest request)
        {
            await _stationLetterService.AddStationLetterFileAsync(id, request.FileNames);
            await _uploadService.CleanFilesAsync(Consts.UploadFile.StationLetter);
            return new OkResult();
        }

        [HttpPut("promotion/{id}")]
        public async Task<IActionResult> AddPromotionFileAsync(string id, [FromForm] UpdateFilesRequest request)
        {
            await _promotionService.AddPromotionFileAsync(id, request.FileNames);
            await _uploadService.CleanFilesAsync(Consts.UploadFile.Promotion);
            return new OkResult();
        }

        [HttpGet]
        public async Task<GetFileNamesResponse> GetFileNamesFromS3Async()
        {
            var keys = await _uploadService.GetObjectsAsync();

            return new() {Valid = true, Names = keys};
        }
    }
}