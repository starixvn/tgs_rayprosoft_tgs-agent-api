using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Services.Operator.GameProviders;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.GameBalance
{
    internal class FundInAllGameBalanceCommandService : ICommandService<FundInAllGameBalanceCommand>
    {
        private readonly ICommandService<FundInGameBalanceCommand> _fundInService;
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGetService<GetGameBalanceQuery, GameBalanceInfo> _getGameBalanceService;

        public FundInAllGameBalanceCommandService(
            IGameProviderRepository gameProviderRepository,
            IGetService<GetGameBalanceQuery, GameBalanceInfo> getGameBalanceService,
            ICommandService<FundInGameBalanceCommand> fundInService)
        {
            _gameProviderRepository =
                gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _getGameBalanceService =
                getGameBalanceService ?? throw new ArgumentNullException(nameof(getGameBalanceService));
            _fundInService = fundInService ?? throw new ArgumentNullException(nameof(fundInService));
        }

        public async ValueTask ExecuteAsync(FundInAllGameBalanceCommand command,
            CancellationToken cancellationToken = default)
        {
            var processGps = _gameProviderRepository
                .QueryAllSubGameProvidersAsync(command.BrandId)
                .Where(gp => gp.IsTransferWallet
                    && gp.Status == Status.Active)
                .Select(gp => gp.GpCode)
                .Distinct();

            await foreach (var gpCode in processGps.ConfigureAwait(false))
            {
                if (gpCode == command.GpCode)
                    continue;

                try
                {
                    var balanceInfo = await _getGameBalanceService
                        .GetAsync(
                            new GetGameBalanceQuery(command.PlayerCode, gpCode),
                            cancellationToken)
                        .ConfigureAwait(false);

                    if (balanceInfo.Error != 0)
                        continue;

                    var fundInAmount = (int)Math.Floor(balanceInfo.Balance);

                    if (fundInAmount > 0)
                    {
                        await _fundInService
                            .ExecuteAsync(new FundInGameBalanceCommand(
                                    gpCode,
                                    command.PlayerCode,
                                    fundInAmount),
                                cancellationToken)
                            .ConfigureAwait(false);
                    }
                }
                catch { }
            }
        }
    }
}