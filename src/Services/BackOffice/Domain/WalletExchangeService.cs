using System;
using System.Threading.Tasks;
using Tgs.Payment;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.WalletService;

namespace TGS.Services.BackOffice.Domain
{
    internal class WalletExchangeService : IWalletExchangeService
    {
        private readonly IAgentService _agentService;
        private readonly IAgentWalletRepository _agentWalletRepository;
        private readonly IPlayerService _playerService;
        private readonly IWalletRepository _walletRepository;

        public WalletExchangeService(
            IPlayerService playerService,
            IAgentService agentService,
            IWalletRepository walletRepository,
            IAgentWalletRepository agentWalletRepository)
        {
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _agentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
            _agentWalletRepository = agentWalletRepository ?? throw new ArgumentNullException(nameof(agentWalletRepository));
        }

        public ValueTask ExchangeAsync(
            Guid brandId,
            UserType userType,
            string userId,
            TransactionTypes transactionType,
            decimal amount)
        {
            switch (userType)
            {
                case UserType.Player:
                    return ExchangePlayerWalletAsync(
                        brandId,
                        userId,
                        transactionType,
                        amount);

                case UserType.Agent:
                    return ExchangeAgentWalletAsync(
                        brandId,
                        Guid.Parse(userId),
                        transactionType,
                        amount);

                default:
                    throw new InvalidOperationException();
            }
        }

        private async ValueTask ExchangeAgentWalletAsync(
            Guid brandId,
            Guid agentId,
            TransactionTypes transactionType,
            decimal amount)
        {
            var agent = await _agentService
                .GetAgentAsync(x => x.Id == agentId)
                .ConfigureAwait(false);

            _ = await _agentWalletRepository.ExchangeAsync(
                brandId,
                agent.DefaultWalletId,
                agentId,
                transactionType,
                amount).ConfigureAwait(false);
        }

        private async ValueTask ExchangePlayerWalletAsync(
                    Guid brandId,
            string playerCode,
            TransactionTypes transactionType,
            decimal amount)
        {
            var player = await _playerService
                .GetPlayerAsync(x => x.PlayerCode == playerCode)
                .ConfigureAwait(false);

            _ = await _walletRepository.ExchangeAsync(
                brandId,
                player.DefaultWalletId,
                playerCode,
                transactionType,
                amount).ConfigureAwait(false);
        }
    }
}
