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
    }
}