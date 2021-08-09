using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Operator.Services.Wallet;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.GameBalance
{
    internal class FundOutBeforeEnterGameCommandService : ICommandService<FundOutBeforeEnterGameCommand>
    {
        private readonly ICommandService<FundOutGameBalanceCommand> _fundOutService;
        private readonly IPlayerRepository _playerRepository;
        private readonly ICommandService<FundInAllGameBalanceCommand> _fundInAllService;
        private readonly IGetService<GetBalanceQuery, BalanceInfo> _getPlayerBalanceService;

        public FundOutBeforeEnterGameCommandService(
            IPlayerRepository playerRepository,
            ICommandService<FundInAllGameBalanceCommand> fundInAllService,
            IGetService<GetBalanceQuery, BalanceInfo> getPlayerBalanceService,
            ICommandService<FundOutGameBalanceCommand> fundInService)
        {
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _fundInAllService = fundInAllService ?? throw new ArgumentNullException(nameof(fundInAllService));
            _getPlayerBalanceService = getPlayerBalanceService ?? throw new ArgumentNullException(nameof(getPlayerBalanceService));
            _fundOutService = fundInService ?? throw new ArgumentNullException(nameof(fundInService));
        }

        public async ValueTask ExecuteAsync(FundOutBeforeEnterGameCommand command, CancellationToken cancellationToken = default)
        {
            var isAutoTransfer = await _playerRepository
                .GetPlayerAutoTransferFlagAsync(command.PlayerCode)
                .ConfigureAwait(false);

            if (!isAutoTransfer)
                return;

            await _fundInAllService
                .ExecuteAsync(new FundInAllGameBalanceCommand(
                    command.BrandId,
                    command.PlayerCode,
                    command.GpCode))
                .ConfigureAwait(false);

            var playerBalance = await _getPlayerBalanceService
                .GetAsync(new GetBalanceQuery(command.PlayerCode))
                .ConfigureAwait(false);

            await _fundOutService
                .ExecuteAsync(new FundOutGameBalanceCommand(
                    command.GpCode,
                    command.PlayerCode,
                    (int)Math.Floor(playerBalance.Main)))
                .ConfigureAwait(false);
        }
    }
}
