using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operator
{
    public record GameProviderModel
    {
        public Guid Id { get; init; }

        public string Code { get; init; }

        public string Name { get; init; }

        public string Description { get; init; }

        public Status Status { get; init; }

        public bool IsTransferWallet { get; init; }

        public GameType[] GameTypes { get; init; }
    }
}