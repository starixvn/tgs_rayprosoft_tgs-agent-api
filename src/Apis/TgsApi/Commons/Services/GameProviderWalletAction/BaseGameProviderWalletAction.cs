using System;
using System.Threading.Tasks;
using Tgs.CashService.Client;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Services.Caches;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Models.GameProviderFundTransferTransaction;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;

namespace TGS.Apis.TgsApi.Commons.Services.GameProviderWalletAction;

public abstract class BaseGameProviderWalletAction : IGameProviderWalletAction
{
    private readonly ICashServiceClient _cashServiceClient;
    private readonly IGameProviderFundTransferTransactionRepository _gameProviderFundTransferTransactionRepository;
    private readonly IGameProviderOperations _gameProviderOperations;
    private readonly IPlayerOperations _playerOperations;

    public BaseGameProviderWalletAction(
        IPlayerOperations playerOperations,
        IGameProviderOperations gameProviderOperations,
        IGameProviderFundTransferTransactionRepository gameProviderFundTransferTransactionRepository,
        ICashServiceClient cashServiceClient)
    {
        _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
        _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
        _gameProviderFundTransferTransactionRepository = gameProviderFundTransferTransactionRepository ?? throw new ArgumentNullException(nameof(gameProviderFundTransferTransactionRepository));
        _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
    }

    public abstract ValueTask<bool> CheckingAmountAsync(CachedPlayer player, decimal amount);

    /// <summary>
    /// Funds the in asynchronous.
    /// Gp -> Tgs
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    public async ValueTask<GameProviderWalletFundResponse> FundInAsync(GameProviderWalletFundRequest request)
    {
        var getGameProvider = await GetGameProviderAsync();
        var amount = Math.Abs(request.Amount);

        if (!getGameProvider.GameProviderCode.IgnoreCaseEquals(request.GameProviderCode))
        {
            throw new ArgumentException($"{request.GameProviderCode} does not match the system setting GameProviderCode");
        }

        var player = await _playerOperations.GetCachedPlayerAsync(request.PlayerCode);
        if (player == null)
        {
            throw new PlayerNotFoundException(request.PlayerCode);
        }

        if (!await CheckingAmountAsync(player, amount).ConfigureAwait(false))
        {
            throw new InvalidAmountException("The transfer amount exceeds the game manufacturer's limit");
        }

        var responseBalance = await GetGameProviderBalanceAsync(player);
        if (!responseBalance.Success)
        {
            throw new PlayerNotFoundException(request.PlayerCode);
        }

        if (responseBalance.Balance - amount < 0)
        {
            throw new PlayerHasInsufficientFundsException();
        }

        /*
         * 建立Game交易單
         */
        var transactionGameProviderId = await _gameProviderFundTransferTransactionRepository.AddAsync(new AddGameProviderFundTransferTransactionRequest()
        {
            TransactionId = null,
            GameProviderId = getGameProvider.GameProviderId,
            WalletId = player.DefaultWalletId,
            GameProviderTransactionId = string.Empty,
            PlayerCode = player.PlayerCode,
            FundSource = FundSource.FundIn,
            FromWallet = getGameProvider.GameProviderCode,
            ToWallet = "Default",
            Amount = request.Amount,
            Balance = 0,
            CreatedOn = DateTimeOffset.UtcNow,
            Description = string.Empty
        });

        var (Success, OrderId, Balance) = await FundInAsync(transactionGameProviderId.Value, player, Math.Abs(request.Amount));

        FundTransferTransactionStatus fundTransferStatus = FundTransferTransactionStatus.FailureOnGameProvider;
        if (Success)
        {
            fundTransferStatus = FundTransferTransactionStatus.Success;
        }

        var transactionsId = default(Guid?);
        var postBalance = default(decimal?);
        var exception = default(Exception);
        if (fundTransferStatus == FundTransferTransactionStatus.Success)
        {
            try
            {
                var wallet = await _cashServiceClient.GameProviderFundInAsync(
                    player.DefaultWalletId,
                    amount,
                    null,
                    null).ConfigureAwait(false);

                fundTransferStatus = FundTransferTransactionStatus.Success;
                transactionsId = wallet.TransactionId;
                postBalance = wallet.Balance;
            }
            catch (Exception ex)
            {
                fundTransferStatus = FundTransferTransactionStatus.FailureOnSystem;
                exception = ex;
            }
        }

        /*
       * 更新Game交易單
       */
        await _gameProviderFundTransferTransactionRepository.UpdateAsync(new UpdateGameProviderFundTransferTransactionRequest
        {
            GameProviderFundTransferTransactionId = transactionGameProviderId.Value,
            TransactionId = transactionsId ?? null,
            GameProviderTransactionId = OrderId,
            Status = fundTransferStatus,
            Balance = postBalance ?? 0,
            UpdatedOn = DateTimeOffset.UtcNow,
            Description = (fundTransferStatus == FundTransferTransactionStatus.Success) ? string.Empty : (exception == null) ? string.Empty : exception.Message
        });

        return new()
        {
            Error = (fundTransferStatus == FundTransferTransactionStatus.Success) ? (int)TgsApiError.None : (int)TgsApiError.GameBalanceFundInFailed,
            ErrorMessage = (fundTransferStatus == FundTransferTransactionStatus.Success) ? string.Empty : fundTransferStatus.Description("GP failed to transfer out of the main wallet.(GP轉出主錢包失敗。)"),
            GameProviderTransactionId = OrderId,
            TransactionsId = transactionsId
        };
    }

    public abstract ValueTask<(bool Success, string OrderId, decimal? Balance)> FundInAsync(Guid transactionGameProviderId, CachedPlayer player, decimal amount);

    /// <summary>
    /// Funds the out asynchronous.
    /// Tgs -> Gp
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    public async ValueTask<GameProviderWalletFundResponse> FundOutAsync(GameProviderWalletFundRequest request)
    {
        var getGameProvider = await GetGameProviderAsync();
        var amount = Math.Abs(request.Amount);

        if (!getGameProvider.GameProviderCode.IgnoreCaseEquals(request.GameProviderCode))
        {
            throw new ArgumentException($"{request.GameProviderCode} does not match the system setting GameProviderCode");
        }

        var player = await _playerOperations.GetCachedPlayerAsync(request.PlayerCode);
        if (player == null)
        {
            throw new PlayerNotFoundException(request.PlayerCode);
        }

        if (!await CheckingAmountAsync(player, amount).ConfigureAwait(false))
        {
            throw new InvalidAmountException("The transfer amount exceeds the game manufacturer's limit");
        }

        var responseBalance = await GetGameProviderBalanceAsync(player);
        if (!responseBalance.Success)
        {
            throw new PlayerNotFoundException(request.PlayerCode);
        }

        var playerBalance = await _playerOperations.GetPlayerBalanceAsync(player.DefaultWalletId, false);

        if (playerBalance - amount < 0)
        {
            throw new PlayerHasInsufficientFundsException();
        }

        /*
        * 建立Game交易單
        */
        var transactionGameProviderId = await _gameProviderFundTransferTransactionRepository.AddAsync(new AddGameProviderFundTransferTransactionRequest()
        {
            TransactionId = null,
            GameProviderId = getGameProvider.GameProviderId,
            WalletId = player.DefaultWalletId,
            GameProviderTransactionId = string.Empty,
            PlayerCode = player.PlayerCode,
            FundSource = FundSource.FundOut,
            FromWallet = "Default",
            ToWallet = getGameProvider.GameProviderCode,
            Amount = amount * -1,
            Balance = 0,
            CreatedOn = DateTimeOffset.UtcNow,
            Description = string.Empty
        });

        #region 扣掉主錢包

        var fundTransferStatus = FundTransferTransactionStatus.FailureOnGameProvider;
        var transactionsId = default(Guid?);
        var postBalance = default(decimal?);
        var exception = default(Exception);

        try
        {
            var wallet = await _cashServiceClient.GameProviderFundOutAsync(
                player.DefaultWalletId,
                amount,
                null,
                null).ConfigureAwait(false);

            fundTransferStatus = FundTransferTransactionStatus.Success;
            transactionsId = wallet.TransactionId;
            postBalance = wallet.Balance;
        }
        catch (Exception ex)
        {
            fundTransferStatus = FundTransferTransactionStatus.FailureOnSystem;
            exception = ex;
        }

        #endregion 扣掉主錢包

        var orderId = default(string);
        if (fundTransferStatus == FundTransferTransactionStatus.Success)
        {
            var (Success, OrderId, Balance) = await FundOutAsync(transactionGameProviderId.Value, player, amount);
            fundTransferStatus = FundTransferTransactionStatus.FailureOnGameProvider;
            if (Success)
            {
                fundTransferStatus = FundTransferTransactionStatus.Success;
                orderId = OrderId;
            }
        }

        await _gameProviderFundTransferTransactionRepository.UpdateAsync(new UpdateGameProviderFundTransferTransactionRequest
        {
            GameProviderFundTransferTransactionId = transactionGameProviderId.Value,
            TransactionId = transactionsId,
            GameProviderTransactionId = orderId,
            Status = fundTransferStatus,
            Balance = postBalance ?? 0,
            UpdatedOn = DateTimeOffset.UtcNow,
            Description = (fundTransferStatus == FundTransferTransactionStatus.Success) ? string.Empty : (exception == null) ? string.Empty : exception.Message
        });

        return new GameProviderWalletFundResponse()
        {
            Error = (fundTransferStatus == FundTransferTransactionStatus.Success)
                ? (int)TgsApiError.None
                : (int)TgsApiError.GameBalanceFundOutFailed,
            ErrorMessage = (fundTransferStatus == FundTransferTransactionStatus.Success)
                ? string.Empty
                : fundTransferStatus.Description("Failed to transfer the main wallet to GP.(主錢包轉入GP失敗。)"),
            GameProviderTransactionId = orderId,
            TransactionsId = transactionsId
        };
    }

    public abstract ValueTask<(bool Success, string OrderId, decimal? Balance)> FundOutAsync(Guid transactionGameProviderId, CachedPlayer player, decimal amount);

    /// <summary>
    /// Gets the balance asynchronous.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    public async ValueTask<GameProviderWalletGetBalanceResponse> GetBalanceAsync(GameProviderWalletGetBalanceRequest request)
    {
        var getGameProvider = await GetGameProviderAsync();

        if (!getGameProvider.GameProviderCode.IgnoreCaseEquals(request.GameProviderCode))
        {
            throw new ArgumentException($"{request.GameProviderCode} does not match the system setting GameProviderCode");
        }

        var player = await _playerOperations.GetCachedPlayerAsync(request.PlayerCode);

        if (player == null)
        {
            throw new PlayerNotFoundException(request.PlayerCode);
        }

        var getBalance = await GetGameProviderBalanceAsync(player);

        return new() { Success = getBalance.Success, Balance = getBalance.Balance };
    }

    public abstract ValueTask<(Guid GameProviderId, string GameProviderCode)> GetGameProviderAsync();

    public abstract ValueTask<(bool Success, decimal Balance)> GetGameProviderBalanceAsync(CachedPlayer player);
}
