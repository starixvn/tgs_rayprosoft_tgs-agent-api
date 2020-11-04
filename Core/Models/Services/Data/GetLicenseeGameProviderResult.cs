using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Data
{
    public record GetLicenseeGameProviderResult
    {
        public Guid Id { get; init; }
        public string Code { get; init; }
        public string Name { get; init; }
        public Status Status { get; init; }
        public string Description { get; init; }
        public GameType GameType { get; init; }
    }
}
