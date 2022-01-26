using System;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Models.LicenseeMarket
{
    public class GetLicenseeMarketsReportRequest : PageableRequest
    {
        public string CurrencyCode { get; init; } = default!;

        public string[]? CurrencyCodes { get; init; }

        public DateTimeOffset From { get; init; }

        public Guid LicenseeId { get; init; }

        public DateTimeOffset To { get; init; }
    }
}