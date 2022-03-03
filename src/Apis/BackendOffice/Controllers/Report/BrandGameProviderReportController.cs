using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using Tgs.Report.Client;
using TGS.Services.BackOffice.Application.Models.Aggregate;
using TGS.Services.BackOffice.Application.Models.BrandGameProvider;
using Tgs.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandGameProviderReportController : BaseContextController
    {
        private readonly IReportClient _client;
        private readonly IMediator _mediator;

        public BrandGameProviderReportController(IReportClient client, IMediator mediator)
        {
            _client = client;
            _mediator = mediator;
        }

        [HttpGet]
        [Description("Get brand game provider round aggregate")]
        public async Task<GetBrandGameProviderRoundsReportResponse> GetGameProviderRoundsAsync(
            [FromServices]
            IGetService<GetBrandGameProviderRoundsReportRequest, GetBrandGameProviderRoundsReportResponse>
                brandGameProviderRoundsService,
            [FromQuery] GetBrandGameProviderRoundsReportRequest request)
            => await brandGameProviderRoundsService
                .GetAsync(request)
                .ConfigureAwait(false);

        [HttpPost("generate")]
        [Description("Generate brand game provider round aggregate")]
        public async Task<MessageResponse> GenerateGameProviderAggregateAsync(
            [FromServices] IGetService<GenerateBrandGameProviderAggregateRequest, MessageResponse>
                generateBrandGameProviderAggregateService,
            [FromBody] GenerateBrandGameProviderAggregateRequest request)
            => await generateBrandGameProviderAggregateService
                .GetAsync(request)
                .ConfigureAwait(false);
    }
}