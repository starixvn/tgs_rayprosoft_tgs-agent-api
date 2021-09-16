using System.IO;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Tgs.Core.Infrastructures.Utility;
using TGS.Services.Report.Application.Models.Market;

namespace TGS.Services.Report.Application.Queries.Market
{
    internal class ExportMarketsQueryHandler : Handler<ExportMarketsRequest, MemoryStream>
    {
        private readonly IMediator _mediator;

        public ExportMarketsQueryHandler(IMediator mediator, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _mediator = mediator;
        }

        public override async Task<MemoryStream> Handle(
            ExportMarketsRequest request,
            CancellationToken cancellationToken)
        {
            var getMarketsRequest = new GetMarketsRequest
            {
                BrandId = request.BrandId,
                From = request.From,
                To = request.To,
                ShowAll = true
            };
            var rr1 = new ResourceReader("Resources1.resources");
            var dict = rr1.GetEnumerator();
            var markets = await _mediator.Send(getMarketsRequest, cancellationToken).ConfigureAwait(false);
            var xlsx = markets.Items.Export("games");
            await using var stream = new MemoryStream();
            xlsx.SaveAs(stream);
            return new MemoryStream(stream.ToArray());
        }
    }
}