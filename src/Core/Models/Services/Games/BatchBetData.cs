using System;

namespace TGS.Core.Models.Services.Games
{
    public record BatchBetData
    {
        private readonly string _mapperKey;

        public Guid GameProviderId { get; init; }
        public string GameProviderCode { get; init; }
        public bool AutoCloseRound { get; init; }
        public string ExternalBatchId { get; init; }
        public bool AllowCancelCancelTransaction { get; init; } = false;
        public bool HasCurrencyFromGameProvider { get; init; } = true;
        public BatchTransactionData[] Transactions { get; init; }

        public string ErrorMapperKey
        {
            get => _mapperKey ?? Consts.Default;
            init => _mapperKey = value;
        }
    }
}