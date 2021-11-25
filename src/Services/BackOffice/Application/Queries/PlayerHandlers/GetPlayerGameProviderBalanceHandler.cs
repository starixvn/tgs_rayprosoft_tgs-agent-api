using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tgs.CashService.Client;
using Tgs.GameProviders.Pluggable.Client;
using TGS.Services.BackOffice.Application.Models.Player;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Queries.PlayerHandlers
{
    public class GetPlayerGameProviderBalanceHandler : Handler<GetPlayerGameProviderBalanceRequest, GetPlayerGameProviderBalanceResponse[]>
    {
        private readonly IBrandGameProviderService _brandGameProviderService;
        private readonly ICashServiceClient _cashServiceClient;
        private readonly IGameProvidersBackofficePluggableClient _gameProvidersBackofficePluggableClient;
        private readonly IPlayerService _playerService;

        public GetPlayerGameProviderBalanceHandler(
               IPlayerService playerService,
               ICashServiceClient cashServiceClient,
               IGameProvidersBackofficePluggableClient gameProvidersBackofficePluggableClient,
               IBrandGameProviderService brandGameProviderService,
               IHttpContextAccessor accessor) :
            base(accessor)
        {
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
            _gameProvidersBackofficePluggableClient = gameProvidersBackofficePluggableClient ?? throw new ArgumentNullException(nameof(gameProvidersBackofficePluggableClient));
            _brandGameProviderService = brandGameProviderService ?? throw new ArgumentNullException(nameof(brandGameProviderService));
        }

        public override async Task<GetPlayerGameProviderBalanceResponse[]> Handle(GetPlayerGameProviderBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var gameProviderBalance = new List<GetPlayerGameProviderBalanceResponse>();
            var player = await _playerService.GetPlayerAsync(p => p.PlayerCode == request.PlayerCode);
            var brandGameProviders = (_brandGameProviderService
                .GetBrandGameProviderQueries(q => q.Where(x => x.BrandId == player.BrandId && x.GameProvider.TransferWallet)))
                .Select(x => new
                {
                    GameProviderCode = x.GameProvider.Code,
                    GameProviderName = x.GameProvider.Name,
                })
                .OrderBy(x => x.GameProviderCode)
                .ToArray();

            var walletBalance = await _cashServiceClient.GetBalanceAsync(player.DefaultWalletId).ConfigureAwait(false);

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
                    try
                    {
                        balance = await _gameProvidersBackofficePluggableClient
                        .GetBalanceAsync(
                            player.PlayerCode,
                            brandGameProvider.GameProviderCode)
                        .ConfigureAwait(false);
                    }
                    finally
                    {
                        gameProviderBalance.Add(new()
                        {
                            IsMainWallet = false,
                            GameProviderCode = brandGameProvider.GameProviderCode,
                            GameProviderName = brandGameProvider.GameProviderName,
                            Balance = balance
                        });
                    }
                }));
            }

            await Task.WhenAll(tasks);

            return gameProviderBalance.ToArray();
        }
    }
}
