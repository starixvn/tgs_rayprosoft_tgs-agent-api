using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages.Game;
using TGS.Apis.BackendOffice.Services;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Game;
using TGS.Services.BackOffice.Application.Queries.Game;
using TGS.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers.GameProvider
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : BaseContextController
    {
        private readonly IMediator _mediator;
        private readonly IUploadService _uploadService;
        private readonly ITgsRepository _tgsRepository;

        public GameController(IMediator mediator, IUploadService uploadService, ITgsRepository tgsRepository)
        {
            _mediator = mediator;
            _uploadService = uploadService;
            _tgsRepository = tgsRepository;
        }

        [HttpGet]
        [Description("Get games")]
        public Task<PagedResponse<GameModel>>
            GetGameProvidersAsync([FromQuery] GetGamesRequest request) => _mediator.Send(request);

        [HttpPost]
        [Description("Create games")]
        public Task<CreateGamesResponse> CreateGamesAsync(CreateGamesRequest request) =>
            _mediator.Send(request);

        [HttpPut("{id}")]
        [Description("Update a game")]
        public Task<MessageResponse> UpdateGameAsync(int id, UpdateGameRequest request)
        {
            request.Id = id;
            return _mediator.Send(request);
        }

        [HttpPost("upload")]
        [Description("Upload games")]
        public Task<UploadGamesResponse> UploadGamesAsync([FromForm] UploadGamesRequest request) =>
            _mediator.Send(request);

        [HttpPost("localization/upload")]
        [Description("Upload game localization file")]
        public async Task<MessageResponse> UploadPlayersAsync(
            IFormFile file,
            [FromServices] IGetService<UpdateGameLocalizationQuery, UpdateGameLocationInfo> updateGameLocalizationCommandService)
        {
            var uploadedResult = await _uploadService.GetUploadedExcelAsync<UploadGameLocalizationModel>(file);

            var games = uploadedResult
                .Where(p => !string.IsNullOrEmpty(p.Id))
                .Select(x => new GameLocalizationModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Localization = new Dictionary<string, JsonDocument>()
                    {
                        { "en-US", JsonDocument.Parse("{ \"name\": \""+x.EnUS+"\" }") },
                        { "zh-CN", JsonDocument.Parse("{ \"name\": \""+x.ZhCN+"\" }") },
                        { "zh-TW", JsonDocument.Parse("{ \"name\": \""+x.ZhTW+"\" }") },
                        { "id-ID", JsonDocument.Parse("{ \"name\": \""+x.IdID+"\" }") },
                        { "th-TH", JsonDocument.Parse("{ \"name\": \""+x.ThTH+"\" }") },
                        { "vi-VN", JsonDocument.Parse("{ \"name\": \""+x.ViVN+"\" }") },
                        { "ko-KR", JsonDocument.Parse("{ \"name\": \""+x.KoKR+"\" }") },
                        { "ja-JP", JsonDocument.Parse("{ \"name\": \""+x.JaJP+"\" }") }
                    }
                })
                .ToList();

            var query = new UpdateGameLocalizationQuery() { Games = games };

            var info = await updateGameLocalizationCommandService.GetAsync(query).ConfigureAwait(false);

            var diffGames = info.DiffGameIds;

            if (diffGames.Any())
            {
                return new MessageResponse()
                {
                    Valid = false,
                    Message = "Games Not Found",
                    Errors = diffGames.Select(x => x.ToString()).ToArray()
                };
            }
            else
            {
                return new MessageResponse
                {
                    Valid = true,
                    Message = "Update Game Localization Success"
                };
            }
        }
    }
}