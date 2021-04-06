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
        private readonly IAmountValidator _amountValidator;
        private readonly IReadOnlyDictionary<GameActionType, TransactionSettings> _map;

        public TransactionSettingsGenerator(IAmountValidator amountValidator)
        {
            _amountValidator = amountValidator;

            _map = new Dictionary<GameActionType, TransactionSettings>
            {
                {GameActionType.PlaceBet, PlaceBetTransactionSettings},
                {GameActionType.WinBet, WinBetTransactionSettings},
                {GameActionType.LoseBet, LoseBetTransactionSettings},
                {GameActionType.FreeBet, FreeBetTransactionSettings},
                {GameActionType.TieBet, TieBetTransactionSettings},
                {GameActionType.CancelTransaction, CancelTransactionSettings},
                {GameActionType.WinJackpot, WinJackpotTransactionSettings}
            };
        }

        private TransactionSettings PlaceBetTransactionSettings => new()
        {
            GameActionType = GameActionType.PlaceBet,
            ValidateAmount = _ => _amountValidator.ThrowIfAmountIsNegative,
            HasWalletUpdate = t => t.Amount > 0,
            MustHaveExistingRound = false,
            MustHaveReferencedTransaction = false,
            GetTransactionAmount = a => -a // Get Transaction Amount - set parameter as negative
        };

        private TransactionSettings WinBetTransactionSettings => new()
        {
            GameActionType = GameActionType.WinBet,
            ValidateAmount = _ => _amountValidator.ThrowIfAmountIsNotPositive,
            HasWalletUpdate = _ => true,
            MustHaveExistingRound = true,
            MustHaveReferencedTransaction = true,
            GetTransactionAmount = a => a // Get Transaction Amount - set parameter as is
        };

        private TransactionSettings LoseBetTransactionSettings => new()
        {
            GameActionType = GameActionType.LoseBet,
            ValidateAmount = _ => _amountValidator.IgnoreAmount,
            HasWalletUpdate = _ => false,
            MustHaveExistingRound = true,
            MustHaveReferencedTransaction = true,
            GetTransactionAmount = _ => 0m // Get Transaction Amount - set parameter as is
        };

        private TransactionSettings FreeBetTransactionSettings => new()
        {
            GameActionType = GameActionType.FreeBet,
            ValidateAmount = _ => _amountValidator.ThrowIfAmountIsNegative,
            HasWalletUpdate = t => t.Amount > 0,
            MustHaveExistingRound = false,
            MustHaveReferencedTransaction = false,
            GetTransactionAmount = a => a // Get Transaction Amount - set parameter as is
        };

        private TransactionSettings TieBetTransactionSettings => new()
        {
            GameActionType = GameActionType.TieBet,
            ValidateAmount = _ => _amountValidator.ThrowIfAmountIsNotPositive,
            HasWalletUpdate = _ => true,
            MustHaveExistingRound = true,
            MustHaveReferencedTransaction = true,
            GetTransactionAmount = a => a // Get Transaction Amount - set parameter as is
        };

        private TransactionSettings CancelTransactionSettings => new()
        {
            GameActionType = GameActionType.CancelTransaction,
            ValidateAmount = _ => _amountValidator.IgnoreAmount,
            HasWalletUpdate = _ => true,
            MustHaveExistingRound = true,
            MustHaveReferencedTransaction = true,
            GetTransactionAmount =
                a => -a // Get Transaction Amount - inverse the sign (it will be set by the referenced amount)
        };

        private TransactionSettings WinJackpotTransactionSettings => new()
        {
            GameActionType = GameActionType.WinJackpot,
            ValidateAmount = _ => _amountValidator.ThrowIfAmountIsNotPositive,
            HasWalletUpdate = _ => true,
            MustHaveExistingRound = false,
            MustHaveReferencedTransaction = false,
            GetTransactionAmount = a => a // Get Transaction Amount - set parameter as is
        };

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