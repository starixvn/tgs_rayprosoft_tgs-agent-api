using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages.GameProvider;
using TGS.Apis.BackendOffice.Services;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.GameProvider;
using TGS.Services.BackOffice.Application.Models.SubGameProvider;
using TGS.Services.BackOffice.Application.Queries.GameProviderHandlers;
using TGS.Services.BackOffice.Application.Queries.GameProviders;
using TGS.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers.GameProvider
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class GameProviderController : BaseContextController
    {
        private readonly IMediator _mediator;
        private readonly IUploadService _uploadService;

        public GameProviderController(IMediator mediator, IUploadService uploadService)
        {
            _mediator = mediator;
            _uploadService = uploadService;
        }

        [HttpGet]
        [Description("Get game providers")]
        public Task<PagedResponse<GameProviderBaseModel>>
            GetGameProvidersAsync([FromQuery] GetGameProvidersRequest request) => _mediator.Send(request);

        [HttpGet("{id}")]
        [Description("Get a game provider")]
        public Task<GetGameProviderResponse> GetGameProviderAsync(Guid id) =>
            _mediator.Send(new GetGameProviderRequest(id));

        [HttpPost]
        [Description("Create a game provider")]
        public Task<MessageResponse> CreateGameProviderAsync(CreateGameProviderRequest request) =>
            _mediator.Send(request);

        [HttpPut("{id}")]
        [Description("Update a game provider")]
        public Task<MessageResponse> UpdateGameProviderAsync(Guid id, UpdateGameProviderRequest request)
        {
            request.Id = id;
            return _mediator.Send(request);
        }

        [HttpGet("localization/export")]
        [Description("Export game providers localization for example")]
        public async Task<FileStreamResult> ExportGameProvidersLocalizationAsync(
            [FromServices] IQueryService<GetAllSupportLanguagesQuery, string> getAllSupportLangService,
            [FromServices] IQueryService<GetGameProvidersLocalizationQuery, SubGameProviderData> queryService,
            [FromServices] IGetService<CreateSubGameProviderExcelStreamQuery, Stream> createExcelService)
        {
            var allSupportLangs = getAllSupportLangService.QueryAsync(new GetAllSupportLanguagesQuery());
            var subGameProviders = queryService.QueryAsync(new GetGameProvidersLocalizationQuery());

            var fileStream = await createExcelService
                .GetAsync(new CreateSubGameProviderExcelStreamQuery(
                    allSupportLangs.ToEnumerable(),
                    subGameProviders.ToEnumerable()))
                .ConfigureAwait(false);

            return new FileStreamResult(
                fileStream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [HttpPost("localization/upload")]
        [Description("Upload game providers localization file")]
        public async Task<MessageResponse> UploadGameProvidersLocalizationAsync(
            IFormFile file,
            [FromServices]
            IGetService<UpdateGameProvidersLocalizationQuery, UpdateGameProvidersLocationInfo>
                updateGameLocalizationQueryService)
        {
            var uploadedResult = await _uploadService.GetUploadedExcelAsync<UploadGameProvidersLocalizationModel>(file);

            var gameProviders = uploadedResult
                .Where(p => !string.IsNullOrEmpty(p.Id))
                .Select(x => new GameProvidersLocalizationModel
                {
                    Id = new Guid(x.Id),
                    Name = x.Name,
                    Localization = new Dictionary<string, JsonDocument>
                    {
                        {"en-US", JsonDocument.Parse("{ \"name\": \"" + x.EnUS + "\" }")},
                        {"zh-CN", JsonDocument.Parse("{ \"name\": \"" + x.ZhCN + "\" }")},
                        {"zh-TW", JsonDocument.Parse("{ \"name\": \"" + x.ZhTW + "\" }")},
                        {"id-ID", JsonDocument.Parse("{ \"name\": \"" + x.IdID + "\" }")},
                        {"th-TH", JsonDocument.Parse("{ \"name\": \"" + x.ThTH + "\" }")},
                        {"vi-VN", JsonDocument.Parse("{ \"name\": \"" + x.ViVN + "\" }")},
                        {"ko-KR", JsonDocument.Parse("{ \"name\": \"" + x.KoKR + "\" }")},
                        {"ja-JP", JsonDocument.Parse("{ \"name\": \"" + x.JaJP + "\" }")}
                    }
                })
                .ToList();

            var query = new UpdateGameProvidersLocalizationQuery
            {
                GameProviders = gameProviders,
                EditorUsername = Username
            };

            var info = await updateGameLocalizationQueryService.GetAsync(query).ConfigureAwait(false);

            var diffGameProviders = info.DiffGameProviderIds;

            if (diffGameProviders.Any())
            {
                return new MessageResponse
                {
                    Valid = false,
                    Message = "GameProviders Not Found",
                    Errors = diffGameProviders.Select(x => x.ToString()).ToArray()
                };
            }

            return new MessageResponse
            {
                Valid = true,
                Message = "Update Game Providers Localization Success"
            };
        }
    }
}