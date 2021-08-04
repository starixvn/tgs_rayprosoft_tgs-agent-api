using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Operator.GameProviders;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.GameBalance
{
    internal class FundOutAllGameBalanceCommandService : ICommandService<FundOutAllGameBalanceCommand>
    {
        private readonly ICommandService<SubmitGameBalanceFundOutCommand> _fundOutService;
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGetService<GetGameBalanceQuery, GameBalanceInfo> _getGameBalanceService;

        public FundOutAllGameBalanceCommandService(
            IGameProviderRepository gameProviderRepository,
            IGetService<GetGameBalanceQuery, GameBalanceInfo> getGameBalanceService,
            ICommandService<SubmitGameBalanceFundOutCommand> fundOutService)
        {
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _getGameBalanceService = getGameBalanceService ?? throw new ArgumentNullException(nameof(getGameBalanceService));
            _fundOutService = fundOutService ?? throw new ArgumentNullException(nameof(fundOutService));
        }

        public async ValueTask ExecuteAsync(FundOutAllGameBalanceCommand command, CancellationToken cancellationToken = default)
        {
            var processGps = _gameProviderRepository
                .QueryAllSubGameProvidersAsync(command.BrandId)
                .Where(gp => gp.IsTransferWallet
                    && gp.Status == Core.Models.Enums.Status.Active)
                .Select(gp => gp.GpCode)
                .Distinct();

            await foreach (var gpCode in processGps.ConfigureAwait(false))
            {
                if (gpCode == command.GpCode)
                    continue;

                try
                {
                    var balance = await _getGameBalanceService
                        .GetAsync(
                            new GetGameBalanceQuery(command.PlayerCode, gpCode))
                        .ConfigureAwait(false);

                    if (balance.Error == 0)
                    {
                        await _fundOutService
                            .ExecuteAsync(new SubmitGameBalanceFundOutCommand(
                                gpCode,
                                command.PlayerCode,
                                (int)Math.Floor(balance.Balance)))
                            .ConfigureAwait(false);
                    }
                }
                catch { }
            }
        }
    }
}
