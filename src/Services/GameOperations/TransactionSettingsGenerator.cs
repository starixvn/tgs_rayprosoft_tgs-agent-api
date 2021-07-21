using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations.Models;
using TGS.Services.Operations.Validators;

namespace TGS.Services.GameOperations
{
    internal interface ITransactionSettingsGenerator
    {
        TransactionSettings GetTransactionSettings(TransactionData transaction);
    }

    internal sealed class TransactionSettingsGenerator : ITransactionSettingsGenerator
    {
        private readonly IAmountValidator _amountValidator;
        private readonly IReadOnlyDictionary<GameActionType, Func<TransactionData, TransactionSettings>> _map;

        public TransactionSettingsGenerator(IAmountValidator amountValidator)
        {
            _amountValidator = amountValidator;

            _map = new Dictionary<GameActionType, Func<TransactionData, TransactionSettings>>
            {
                {GameActionType.PlaceBet, GetPlaceBetTransactionSettings},
                {GameActionType.WinBet, GetWinBetTransactionSettings},
                {GameActionType.LoseBet, GetLoseBetTransactionSettings},
                {GameActionType.FreeBet, GetFreeBetTransactionSettings},
                {GameActionType.TieBet, GetTieBetTransactionSettings},
                {GameActionType.CancelTransaction, GetCancelTransactionSettings},
                {GameActionType.LockAmount, GetLockAmountTransactionSettings},
                {GameActionType.UnlockAmount, GetUnlockAmountTransactionSettings},
                {GameActionType.WinJackpot, GetWinJackpotTransactionSettings}
            };
        }

        TransactionSettings ITransactionSettingsGenerator.GetTransactionSettings(TransactionData transaction)
        {
            if (!_map.TryGetValue(transaction.GameActionType, out var func))
            {
                throw new InvalidOperationException(
                    $"Don't have transaction settings for GameActionType '{transaction.GameActionType}'");
            }

            return func(transaction);
        }

        private TransactionSettings GetPlaceBetTransactionSettings(TransactionData trx) => new()
        {
            GameActionType = GameActionType.PlaceBet,
            ValidateAmount = _amountValidator.ThrowIfAmountIsNegative,
            HasWalletUpdate = trx.Amount > 0,
            MustHaveExistingRound = false,
            MustHaveReferencedTransaction = false,
            GetTransactionAmount = a => -a // Get Transaction Amount - set parameter as negative
        };

        private TransactionSettings GetWinBetTransactionSettings(TransactionData trx) => new()
        {
            GameActionType = GameActionType.WinBet,
            ValidateAmount = _amountValidator.ThrowIfAmountIsNotPositive,
            HasWalletUpdate = true,
            MustHaveExistingRound = true,
            MustHaveReferencedTransaction = true,
            GetTransactionAmount = a => a // Get Transaction Amount - set parameter as is
        };

        private TransactionSettings GetLoseBetTransactionSettings(TransactionData trx) => new()
        {
            GameActionType = GameActionType.LoseBet,
            ValidateAmount =
                trx.AllowLoseBetNegativeAmount
                    ? _amountValidator.ThrowIfAmountIsPositive
                    : _amountValidator.IgnoreAmount,
            HasWalletUpdate = trx.Amount < 0,
            MustHaveExistingRound = true,
            MustHaveReferencedTransaction = true,
            GetTransactionAmount =
                trx.AllowLoseBetNegativeAmount
                    ? a => a
                    : _ => 0 // Get Transaction Amount - set parameter by AllowLoseBetNegativeAmount setting
        };

        private TransactionSettings GetFreeBetTransactionSettings(TransactionData trx) => new()
        {
            GameActionType = GameActionType.FreeBet,
            ValidateAmount = _amountValidator.ThrowIfAmountIsNegative,
            HasWalletUpdate = trx.Amount > 0,
            MustHaveExistingRound = false,
            MustHaveReferencedTransaction = false,
            GetTransactionAmount = a => a // Get Transaction Amount - set parameter as is
        };

        private TransactionSettings GetTieBetTransactionSettings(TransactionData trx) => new()
        {
            GameActionType = GameActionType.TieBet,
            ValidateAmount = _amountValidator.ThrowIfAmountIsNotPositive,
            HasWalletUpdate = true,
            MustHaveExistingRound = true,
            MustHaveReferencedTransaction = true,
            GetTransactionAmount = a => a // Get Transaction Amount - set parameter as is
        };

        private TransactionSettings GetCancelTransactionSettings(TransactionData trx) => new()
        {
            GameActionType = GameActionType.CancelTransaction,
            ValidateAmount = _amountValidator.IgnoreAmount,
            HasWalletUpdate = true,
            MustHaveExistingRound = true,
            MustHaveReferencedTransaction = true,
            GetTransactionAmount =
                a => -a // Get Transaction Amount - inverse the sign (it will be set by the referenced amount)
        };

        private TransactionSettings GetLockAmountTransactionSettings(TransactionData trx) => new()
        {
            GameActionType = GameActionType.LockAmount,
            ValidateAmount = _amountValidator.ThrowIfAmountIsNotPositive,
            HasWalletUpdate = true,
            MustHaveExistingRound = false,
            MustHaveReferencedTransaction = false,
            GetTransactionAmount = a => -a // Get Transaction Amount - set parameter as negative
        };

        private TransactionSettings GetUnlockAmountTransactionSettings(TransactionData trx) => new()
        {
            GameActionType = GameActionType.UnlockAmount,
            ValidateAmount = _amountValidator.ThrowIfAmountIsNotPositive,
            HasWalletUpdate = true,
            MustHaveExistingRound = true,
            MustHaveReferencedTransaction = false,
            GetTransactionAmount = a => a // Get Transaction Amount - set parameter as is
        };

        private TransactionSettings GetWinJackpotTransactionSettings(TransactionData trx) => new()
        {
            GameActionType = GameActionType.WinJackpot,
            ValidateAmount = _amountValidator.ThrowIfAmountIsNotPositive,
            HasWalletUpdate = true,
            MustHaveExistingRound = false,
            MustHaveReferencedTransaction = false,
            GetTransactionAmount = a => a // Get Transaction Amount - set parameter as is
        };
    }
}