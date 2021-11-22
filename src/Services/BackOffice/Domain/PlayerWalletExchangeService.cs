using System;
using System.Threading.Tasks;
using Tgs.CashService.Client;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Domain
{
    internal class PlayerWalletExchangeService : IPlayerWalletExchangeService
    {
        private readonly ICashServiceClient _cashServiceClient;
        private readonly IPlayerService _playerService;

        public PlayerWalletExchangeService(
            IPlayerService playerService,
            ICashServiceClient cashServiceClient)
        {
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
        }

        public async ValueTask<TransactionResponse> AdjustAsync(
            string playerCode,
            decimal amount,
            string description,
            string externalId,
            Guid createdByUserId)
        {
            var player = await _playerService
                .GetPlayerAsync(x => x.PlayerCode == playerCode)
                .ConfigureAwait(false);

            return await _cashServiceClient.AdjustAsync(
                player.DefaultWalletId,
                amount,
                description,
                externalId,
                createdByUserId).ConfigureAwait(false);
        }

        public async ValueTask<TransactionResponse> BonusAsync(string playerCode, decimal amount, string description, string externalId)
        {
            var player = await _playerService
                .GetPlayerAsync(x => x.PlayerCode == playerCode)
                .ConfigureAwait(false);

            return await _cashServiceClient.BonusAsync(
                player.DefaultWalletId,
                amount,
                description,
                externalId).ConfigureAwait(false);
        }

        public async ValueTask<TransactionResponse> CancelWithdrawalAsync(
            string playerCode,
            decimal amount,
            string description,
            string externalId,
            Guid? createdByUserId)
        {
            var player = await _playerService
                .GetPlayerAsync(x => x.PlayerCode == playerCode)
                .ConfigureAwait(false);

            return await _cashServiceClient.CancelWithdrawalAsync(
                player.DefaultWalletId,
                amount,
                description,
                externalId,
                createdByUserId).ConfigureAwait(false);
        }

        public async ValueTask<TransactionResponse> DepositAsync(string playerCode, decimal amount, string description, string externalId)
        {
            var player = await _playerService
                .GetPlayerAsync(x => x.PlayerCode == playerCode)
                .ConfigureAwait(false);

            return await _cashServiceClient.DepositAsync(
                player.DefaultWalletId,
                amount,
                description,
                externalId).ConfigureAwait(false);
        }

        public async ValueTask<TransactionResponse> GameProviderDepositAsync(string playerCode, decimal amount, string description, string externalId)
        {
            var player = await _playerService
                .GetPlayerAsync(x => x.PlayerCode == playerCode)
                .ConfigureAwait(false);

            return await _cashServiceClient.GameProviderDepositAsync(
                player.DefaultWalletId,
                amount,
                description,
                externalId).ConfigureAwait(false);
        }

        public async ValueTask<TransactionResponse> GameProviderFundInAsync(
            string playerCode,
            decimal amount,
            string description,
            string externalId)
        {
            var player = await _playerService
                .GetPlayerAsync(x => x.PlayerCode == playerCode)
                .ConfigureAwait(false);

            return await _cashServiceClient.GameProviderFundInAsync(
                player.DefaultWalletId,
                amount,
                description,
                externalId).ConfigureAwait(false);
        }

        public async ValueTask<TransactionResponse> GameProviderFundOutAsync(string playerCode, decimal amount, string description, string externalId)
        {
            var player = await _playerService
                .GetPlayerAsync(x => x.PlayerCode == playerCode)
                .ConfigureAwait(false);

            return await _cashServiceClient.GameProviderFundOutAsync(
                player.DefaultWalletId,
                amount,
                description,
                externalId).ConfigureAwait(false);
        }

        public async ValueTask<TransactionResponse> GameProviderWithdrawalAsync(
            string playerCode,
            decimal amount,
            string description,
            string externalId)
        {
            var player = await _playerService
                .GetPlayerAsync(x => x.PlayerCode == playerCode)
                .ConfigureAwait(false);

            return await _cashServiceClient.GameProviderWithdrawalAsync(
                player.DefaultWalletId,
                amount,
                description,
                externalId).ConfigureAwait(false);
        }

        public async ValueTask<decimal> GetBalanceAsync(string playerCode)
        {
            var player = await _playerService
                .GetPlayerAsync(x => x.PlayerCode == playerCode)
                .ConfigureAwait(false);

            return await _cashServiceClient
                .GetBalanceAsync(player.DefaultWalletId)
                .ConfigureAwait(false);
        }

        public async ValueTask<TransactionResponse> OtherAsync(
            string playerCode,
            decimal amount,
            string description,
            string externalId,
            Guid createdByUserId)
        {
            var player = await _playerService
                .GetPlayerAsync(x => x.PlayerCode == playerCode)
                .ConfigureAwait(false);

            return await _cashServiceClient.OtherAsync(
                player.DefaultWalletId,
                amount,
                description,
                externalId,
                createdByUserId).ConfigureAwait(false);
        }

        public async ValueTask<TransactionResponse> PromotionAsync(
            string playerCode,
            decimal amount,
            string description,
            string externalId,
            Guid? createByUserId)
        {
            var player = await _playerService
                .GetPlayerAsync(x => x.PlayerCode == playerCode)
                .ConfigureAwait(false);

            return await _cashServiceClient.PromotionAsync(
                player.DefaultWalletId,
                amount,
                description,
                externalId,
                createByUserId).ConfigureAwait(false);
        }

        public async ValueTask<TransactionResponse> RebateAsync(string playerCode, decimal amount, string description, string externalId)
        {
            var player = await _playerService
                .GetPlayerAsync(x => x.PlayerCode == playerCode)
                .ConfigureAwait(false);

            return await _cashServiceClient.RebateAsync(
                player.DefaultWalletId,
                amount,
                description,
                externalId).ConfigureAwait(false);
        }

        public async ValueTask<TransactionResponse> WithdrawalAsync(string playerCode, decimal amount, string description, string externalId)
        {
            var player = await _playerService
                .GetPlayerAsync(x => x.PlayerCode == playerCode)
                .ConfigureAwait(false);

            return await _cashServiceClient.WithdrawalAsync(
                player.DefaultWalletId,
                amount,
                description,
                externalId).ConfigureAwait(false);
        }
    }
}
