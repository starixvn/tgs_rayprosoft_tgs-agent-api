using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;
using TGS.Services.GameOperations.Models;
using TGS.Services.Operations.Validators;

namespace TGS.Services.GameOperations
{
    internal interface ITransactionSettingsGenerator
    {
        TransactionSettings GetTransactionSettings(GameActionType type);
    }

    internal sealed class TransactionSettingsGenerator : ITransactionSettingsGenerator
    {
        private readonly IReadOnlyDictionary<GameActionType, TransactionSettings> _map;
        private readonly IAmountValidator _amountValidator;

        private TransactionSettings PlaceBetTransactionSettings => new()
        {
            GameActionType = GameActionType.PlaceBet,
            ValidateAmount = t => _amountValidator.ThrowIfAmountIsNotPositive,
            HasWalletUpdate = t => true,
            MustHaveExistingRound = false,
            MustHaveReferencedTransaction = false,
            GetTransactionAmount = a => -a      // Get Transaction Amount - set parameter as negative
        };

        private TransactionSettings WinBetTransactionSettings => new TransactionSettings
        {
            GameActionType = GameActionType.WinBet,
            ValidateAmount = t => _amountValidator.ThrowIfAmountIsNotPositive,
            HasWalletUpdate = t => true,
            MustHaveExistingRound = true,
            MustHaveReferencedTransaction = true,
            GetTransactionAmount = a => a      // Get Transaction Amount - set parameter as is
        };

        private TransactionSettings LoseBetTransactionSettings => new TransactionSettings
        {
            GameActionType = GameActionType.LoseBet,
            ValidateAmount = t => _amountValidator.IgnoreAmount,
            HasWalletUpdate = t => false,
            MustHaveExistingRound = true,
            MustHaveReferencedTransaction = true,
            GetTransactionAmount = a => 0m      // Get Transaction Amount - set parameter as is
        };

        private TransactionSettings FreeBetTransactionSettings => new TransactionSettings
        {
            GameActionType = GameActionType.FreeBet,
            ValidateAmount = t => _amountValidator.ThrowIfAmountIsNegative,
            HasWalletUpdate = t => t.Amount > 0,
            MustHaveExistingRound = false,
            MustHaveReferencedTransaction = false,
            GetTransactionAmount = a => a      // Get Transaction Amount - set parameter as is
        };

        private TransactionSettings TieBetTransactionSettings => new TransactionSettings
        {
            GameActionType = GameActionType.TieBet,
            ValidateAmount = t => _amountValidator.ThrowIfAmountIsNotPositive,
            HasWalletUpdate = t => true,
            MustHaveExistingRound = true,
            MustHaveReferencedTransaction = true,
            GetTransactionAmount = a => a      // Get Transaction Amount - set parameter as is
        };

        private TransactionSettings CancelTransactionSettings => new TransactionSettings
        {
            GameActionType = GameActionType.CancelTransaction,
            ValidateAmount = t => _amountValidator.IgnoreAmount,
            HasWalletUpdate = t => true,
            MustHaveExistingRound = true,
            MustHaveReferencedTransaction = true,
            GetTransactionAmount = a => -a      // Get Transaction Amount - inverse the sign (it will be set by the referenced amount)
        };

        public TransactionSettingsGenerator(IAmountValidator amountValidator)
        {
            _amountValidator = amountValidator;

            _map = new Dictionary<GameActionType, TransactionSettings>
            {
                { GameActionType.PlaceBet, PlaceBetTransactionSettings },
                { GameActionType.WinBet, WinBetTransactionSettings },
                { GameActionType.LoseBet, LoseBetTransactionSettings },
                { GameActionType.FreeBet, FreeBetTransactionSettings },
                { GameActionType.TieBet, TieBetTransactionSettings },
                { GameActionType.CancelTransaction, CancelTransactionSettings }
            };
        }

        TransactionSettings ITransactionSettingsGenerator.GetTransactionSettings(GameActionType type)
        {
            if (!_map.TryGetValue(type, out var settings))
            {
                throw new InvalidOperationException($"Don't have transaction settings for GameActionType '{type}'");
            }

            return settings;
        }
    }
}