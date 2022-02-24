using System;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Models.GameProvider
{
    public class GetGameProviderRoundsReportRequest : PageableRequest
    {
        public DateTimeOffset From { get; init; }

        public DateTimeOffset To { get; init; }

        public Guid BrandId { get; init; }

        public string GameProviderCode { get; init; } = default!;

        public GameType? GameType { get; init; }
    }
}