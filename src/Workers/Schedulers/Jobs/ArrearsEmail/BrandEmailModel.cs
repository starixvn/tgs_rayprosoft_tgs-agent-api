using System;

namespace TGS.Workers.Schedulers.Jobs.ArrearsEmail;

public class BrandEmailModel
{
    public Guid BrandId { get; init; }

    public string Name { get; init; }

    public string OperationalEmail { get; init; }

    public decimal? Percentage { get; init; }

    public decimal NetProfitAndPay { get; init; }

    public string Subject { get; init; }

    public string Content { get; init; }
}