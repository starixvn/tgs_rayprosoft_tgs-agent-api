using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.CashService.Client;
using Tgs.GameProviders.Pluggable.Client;
using TGS.Services.BackOffice.Application.Models.Player;
using TGS.Services.BackOffice.Infrastructure.DataService;
using Tgs.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Queries.PlayerHandlers
{
    public class GetPlayerGameProviderBalanceHandler : IGetService<GetPlayerGameProviderBalanceRequest,
        GetPlayerGameProviderBalanceResponse[]>
    {
        private readonly IBrandGameProviderService _brandGameProviderService;
        private readonly ICashServiceClient _cashServiceClient;
        private readonly IGameProvidersBackofficePluggableClient _gameProvidersBackofficePluggableClient;
        private readonly IPlayerService _playerService;

        public GetPlayerGameProviderBalanceHandler(
            IPlayerService playerService,
            ICashServiceClient cashServiceClient,
            IGameProvidersBackofficePluggableClient gameProvidersBackofficePluggableClient,
            IBrandGameProviderService brandGameProviderService)
        {
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
            _gameProvidersBackofficePluggableClient = gameProvidersBackofficePluggableClient ??
                throw new ArgumentNullException(nameof(gameProvidersBackofficePluggableClient));
            _brandGameProviderService = brandGameProviderService ??
                throw new ArgumentNullException(nameof(brandGameProviderService));
        }

        public async ValueTask<GetPlayerGameProviderBalanceResponse[]> GetAsync(
            GetPlayerGameProviderBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var gameProviderBalance = new List<GetPlayerGameProviderBalanceResponse>();
            var player = await _playerService.GetPlayerAsync(p => p.PlayerCode == request.PlayerCode);
            var brandGameProviders = _brandGameProviderService
                .GetBrandGameProviderQueries(q => q.Where(x =>
                    x.BrandId == player.BrandId &&
                    x.GameProvider.TransferWallet &&
                    x.GameProvider.Currencies
                        .Where(Currency => Currency.CurrencyCode.Contains(player.DefaultWalletCurrencyCode)).Count() >
                    0))
                .Select(x => new
                {
                    GameProviderCode = x.GameProvider.Code,
                    GameProviderName = x.GameProvider.Name,
                    GameProviderCurrencies = x.GameProvider.Currencies
                })
                .OrderBy(x => x.GameProviderCode)
                .ToArray();

            var walletBalance = default(decimal?);
            try
            {
                walletBalance = await _cashServiceClient.GetBalanceAsync(player.DefaultWalletId).ConfigureAwait(false);
            }
            catch
            {
                walletBalance = null;
            }

            gameProviderBalance.Add(new()
            {
                IsMainWallet = true,
                GameProviderCode = "DEFAULT",
                GameProviderName = "DEFAULT",
                Balance = walletBalance
            });

            var tasks = new List<Task>();

            foreach (var brandGameProvider in brandGameProviders)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var balance = default(decimal?);

                    balance = await _gameProvidersBackofficePluggableClient
                        .GetBalanceAsync(
                            player.PlayerCode,
                            brandGameProvider.GameProviderCode)
                        .ConfigureAwait(false);

                    gameProviderBalance.Add(new()
                    {
                        IsMainWallet = false,
                        GameProviderCode = brandGameProvider.GameProviderCode,
                        GameProviderName = brandGameProvider.GameProviderName,
                        Balance = balance
                    });
                }));
            }

            await Task.WhenAll(tasks);

            return gameProviderBalance.ToArray();
        }
    }
}