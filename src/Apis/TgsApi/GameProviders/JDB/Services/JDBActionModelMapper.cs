using System;
using System.Linq;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.JDB.Enums;
using TGS.Shared.Game.Models.JDB.Incoming;
using static TGS.Core.Models.Enums.GameActionType;

namespace TGS.Apis.TgsApi.GameProviders.JDB.Services
{
    public interface IJDBActionModelMapper
    {
        PlayerValidationData Request(string playerCode, PlatformType platformType);

        BetData Request(string playerCode, GameProviderConfigurationData data, FundTransferRequest r);
    }

    public sealed class JDBActionModelMapper : IJDBActionModelMapper
    {
        private static readonly string GameProviderCode = Consts.GameProvider.Habanero;

        PlayerValidationData IJDBActionModelMapper.Request(string playerCode, PlatformType platformType) =>
            new()
            {
                GameProviderCode = GameProviderCode,
                PlayerCode = playerCode,
                PlatformType = platformType
            };

        BetData IJDBActionModelMapper.Request(string playerCode, GameProviderConfigurationData data,
            FundTransferRequest r)
        {
            string currencyCode;
            TransactionData[] transactions;

            if (r.Detail.IsCancel)
            {
                var trx = r.Detail.FundTransfer.CancelTransaction;
                var clientRoundId = r.Detail.ClientRoundId.ToString();

                currencyCode = trx.CurrencyCode;
                transactions = new TransactionData[]
                {
                    new()
                    {
                        Amount = Math.Abs(trx.Amount),
                        ExternalTransactionId = trx.TransactionId,
                        ExternalTransactionReferenceId = trx.TransactionReferenceId,
                        ExternalGameActionGroupId = clientRoundId,
                        ExternalBetId = r.Detail.RoundId,
                        ExternalRoundId = clientRoundId,
                        ExternalGameId = r.Game.ExternalId,
                        GameActionType = GameActionType.CancelTransaction,
                        TimeStamp = DateTimeOffset.Parse(trx.Timestamp),
                        RoundClosed = trx.GameState is GameState.Completing or GameState.Expired
                    }
                };
            }
            else
            {
                var trxs = r.Detail.FundTransfer.Transactions;
                currencyCode = trxs[0].CurrencyCode;

                transactions = trxs.Select(t =>
                {
                    var clientRoundId = r.Detail.ClientRoundId.ToString();

                    return new TransactionData
                    {
                        Amount = Math.Abs(t.Amount),
                        ExternalTransactionId = t.TransactionId,
                        ExternalTransactionReferenceId = t.IsPlaceBet ? null : t.TransactionReferenceId,
                        ExternalGameActionGroupId = clientRoundId,
                        ExternalBetId = r.Detail.RoundId,
                        ExternalRoundId = clientRoundId,
                        ExternalGameId = r.Game.ExternalId,
                        GameActionType =
                            t.IsPlaceBet                              ? PlaceBet :
                            r.Detail.ActionType == ActionType.FreeBet ? FreeBet :
                            t.Amount == 0                             ? LoseBet : WinBet,
                        TimeStamp = DateTimeOffset.Parse(t.Timestamp),
                        RoundClosed = t.GameState is GameState.Completing or GameState.Expired
                    };
                }).ToArray();
            }

            return GetBetData(playerCode, currencyCode, data, transactions);
        }

        #region Private Methods

        private static BetData GetBetData(string playerCode, string currencyCode, GameProviderConfigurationData data,
            TransactionData[] transactions) => new()
        {
            GameProviderId = data.GameProviderId,
            GameProviderConfigurationId = data.GameProviderConfigurationId,
            GameProviderCode = GameProviderCode,
            AutoCloseRound = data.AutoCloseRound,
            PlayerCode = playerCode,
            PlatformType = data.PlatformType,
            HasCurrencyFromGameProvider = true,
            CurrencyCode = currencyCode,
            Transactions = transactions
        };

        

        #endregion
    }
}