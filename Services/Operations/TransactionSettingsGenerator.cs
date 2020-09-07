using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;
using TGS.Services.Operations.Models;

namespace TGS.Services.Operations
{
    internal interface ITransactionSettingsGenerator
    {
        TransactionSettings GetTransactionSettings(GameActionType type);
    }

    internal sealed class TransactionSettingsGenerator : ITransactionSettingsGenerator
    {
        //private TransactionSettings PlaceBetTransactionSettings = new() { 
        //    GameActionType = GameActionType.PlaceBet,
        //    ValidateAmount= t => t.PlaceBetZeroAmountAllowed ? (Action<decimal>)ThrowIfAmountIsNegative : ThrowIfAmountIsNotPositive,
        //    HasWalletUpdate = t => true,
        //    MustHaveExistingRound = false,
        //    MustHaveReferencedTransaction = false,
        //    GetTransactionAmount = a => -a      // Get Transaction Amount - set parameter as negative

        //};

        private readonly IReadOnlyDictionary<GameActionType, Func<TransactionSettings>> _map = new Dictionary<GameActionType, Func<TransactionSettings>>
        {
            { GameActionType.PlaceBet, () => new TransactionSettings()  }
            //{GameActionType.WinBet, WinTransactionSettings},
            //{GameActionType.LoseBet, LoseTransactionSettings},
            //{GameActionType.FreeBet, FreeBetTransactionSettings},
            //{GameActionType.TieBet, TieTransactionSettings}
        };

        TransactionSettings ITransactionSettingsGenerator.GetTransactionSettings(GameActionType type)
        {
            if (!_map.TryGetValue(type, out var func))
            {
                throw new InvalidOperationException($"Don't have transaction settings for GameActionType '{type}'");
            }

            return func();
        }
    }
}