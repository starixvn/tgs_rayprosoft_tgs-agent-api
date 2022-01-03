using System;
using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.GameProvider
{
    public class GetGameProviderRoundsRequest : PageableRequest, IRequest<GetGameProviderRoundsResponse>
    {
        public DateTimeOffset From { get; set; }

        public DateTimeOffset To { get; set; }

        public Guid BrandId { get; set; }

        public string GameProviderCode { get; set; }

        public GameType? GameType { get; set; }
    }
}