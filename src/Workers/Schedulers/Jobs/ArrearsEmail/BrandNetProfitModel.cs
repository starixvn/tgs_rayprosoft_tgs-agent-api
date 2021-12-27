using System;

namespace TGS.Workers.Schedulers.Jobs.ArrearsEmail
{
    public class BrandNetProfitModel
    {
        public Guid BrandId { get; init; }

        public string Name { get; init; }

        public string OperationalEmail { get; init; }

        public decimal? PercentageKey { get; init; }

        public decimal NetProfitAndPay { get; init; }
    }
}