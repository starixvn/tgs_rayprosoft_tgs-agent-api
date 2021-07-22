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
using TGS.Apis.BackendOffice.Controllers.Messages.SubGameProvider;
using TGS.Apis.BackendOffice.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.SubGameProvider;
using TGS.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers.GameProvider
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubGameProviderController : BaseContextController
    {
        private readonly IMediator _mediator;
        private readonly IUploadService _uploadService;

        public SubGameProviderController(IMediator mediator, IUploadService uploadService)
        {
            _mediator = mediator;
            _uploadService = uploadService;
        }

        [HttpGet]
        [Description("Get sub game providers")]
        public Task<PagedResponse<SubGameProviderModel>>
            GetSubGameProvidersAsync([FromQuery] GetSubGameProvidersRequest request) => _mediator.Send(request);

        [HttpGet("{code}")]
        [Description("Get a sub game provider")]
        public Task<GetSubGameProviderResponse> GetSubGameProviderAsync(string code) =>
            _mediator.Send(new GetSubGameProviderRequest(code));

        [HttpPost]
        [Description("Create a sub game provider")]
        public Task<MessageResponse> CreateGameProviderAsync(CreateSubGameProviderRequest request) =>
            _mediator.Send(request);

        [HttpPut("{id}/{code}")]
        [Description("Update a sub game provider")]
        public Task<MessageResponse> UpdateGameProviderAsync(Guid id, string code, UpdateSubGameProviderRequest request)
        {
            request.GameProviderId = id;
            request.Code = code;
            return _mediator.Send(request);
        }

        [HttpGet("localization/export")]
        [Description("Export sub game providers localization for example")]
        public async Task<FileStreamResult> ExportSubGameProvidersLocalizationAsync(
            [FromServices] IGetService<GetSubGameProvidersLocalizationRequest, SubGameProvidersLocalizationInfo>
                getLocalizationQueryService
        )
        {
            var info = await getLocalizationQueryService.GetAsync(new GetSubGameProvidersLocalizationRequest())
                .ConfigureAwait(false);

            var memoryStream = new MemoryStream();

            using (var document = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                var sheets = workbookPart.Workbook.AppendChild(new Sheets());

                sheets.Append(new Sheet
                {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "SubGameProvider"
                });

                var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                var row = new Row();
                row.Append(
                    new Cell
                    {
                        CellValue = new CellValue("Code"),
                        DataType = CellValues.String
                    },
                    new Cell
                    {
                        CellValue = new CellValue("Name"),
                        DataType = CellValues.String
                    }
                );

                foreach (var item in info.Languages)
                {
                    row.Append(
                        new Cell
                        {
                            CellValue = new CellValue(item.Replace("-", "")),
                            DataType = CellValues.String
                        }
                    );
                }

                sheetData.AppendChild(row);

                foreach (var gameProvider in info.GameProviders)
                {
                    row = new Row();
                    row.Append(
                        new Cell
                        {
                            CellValue = new CellValue(gameProvider.SubGameProviderCode),
                            DataType = CellValues.String
                        },
                        new Cell
                        {
                            CellValue = new CellValue(gameProvider.SubGameProviderName),
                            DataType = CellValues.String
                        }
                    );
                    if (gameProvider.Localization != null)
                    {
                        foreach (var item in info.Languages)
                        {
                            var localization = JsonDocument.Parse(gameProvider.Localization);
                            JsonElement lang;
                            var isExist = localization.RootElement.TryGetProperty(item, out lang);
                            if (isExist)
                            {
                                row.Append(
                                    new Cell
                                    {
                                        CellValue = new CellValue(lang.GetProperty("name").ToString()),
                                        DataType = CellValues.String
                                    }
                                );
                            }
                        }
                    }

                    sheetData.AppendChild(row);
                }
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(memoryStream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [HttpPost("localization/upload")]
        [Description("Upload sub game providers localization file")]
        public async Task<MessageResponse> UploadSubGameProvidersLocalizationAsync(
            IFormFile file,
            [FromServices] IGetService<UpdateSubGameProvidersLocalizationRequest, UpdateSubGameProvidersLocationInfo>
                queryService)
        {
            var uploadedResult =
                await _uploadService.GetUploadedExcelAsync<UploadSubGameProvidersLocalizationModel>(file);

            var gameProviders = uploadedResult
                .Where(p => p.Code.IsNotNullOrEmpty())
                .Select(x => new SubGameProvidersLocalizationModel
                {
                    Code = x.Code,
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

            var query = new UpdateSubGameProvidersLocalizationRequest
            {
                SubGameProviders = gameProviders,
                EditorUsername = Username
            };

            var info = await queryService.GetAsync(query).ConfigureAwait(false);

            var diffGameProviders = info.DiffGameProviderCodes;

            if (diffGameProviders.Any())
            {
                return new MessageResponse
                {
                    Valid = false,
                    Message = "Sub GameProviders Not Found",
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