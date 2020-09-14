using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operations.Models.Data
{
    internal record RoundIdAndStatusData
    {
        internal Guid RoundId { get; init; }
        internal string ExternalRoundId { get; init; }
        internal RoundStatus Status { get; init; }
    }
}