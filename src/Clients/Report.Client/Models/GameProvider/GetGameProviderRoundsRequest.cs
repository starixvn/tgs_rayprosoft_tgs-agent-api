using System;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Clients.Report.Client.Models.GameProvider
{
    public class GetGameProviderRoundsRequest : PageableRequest
    {
        public DateTimeOffset From { get; set; }

        public DateTimeOffset To { get; set; }

        public Guid BrandId { get; set; }

        public string[] GameProviderCodes { get; set; }

        public GameType GameType { get; set; }
    }
}