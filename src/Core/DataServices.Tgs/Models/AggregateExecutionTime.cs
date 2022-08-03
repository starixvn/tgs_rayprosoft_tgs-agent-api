using System;

namespace TGS.Core.DataServices.Tgs.Models;

public class AggregateExecutionTime
{
    public short AggregateType { get; set; }

    public DateTimeOffset From { get; set; }

    public DateTimeOffset To { get; set; }
}