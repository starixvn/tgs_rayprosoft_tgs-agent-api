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
        private readonly IGetService<GetBalanceQuery, BalanceInfo> _getPlayerBalanceService;
        private readonly IPlayerRepository _playerRepository;

        public FundOutBeforeEnterGameCommandService(
            IPlayerRepository playerRepository,
            IGetService<GetBalanceQuery, BalanceInfo> getPlayerBalanceService,
            ICommandService<FundOutGameBalanceCommand> fundInService)
        {
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _getPlayerBalanceService = getPlayerBalanceService ??
                throw new ArgumentNullException(nameof(getPlayerBalanceService));
            _fundOutService = fundInService ?? throw new ArgumentNullException(nameof(fundInService));
        }

        public async ValueTask ExecuteAsync(FundOutBeforeEnterGameCommand command,
            CancellationToken cancellationToken = default)
        {
            var isAutoTransfer = await _playerRepository
                .GetPlayerAutoTransferFlagAsync(command.PlayerCode)
                .ConfigureAwait(false);

            if (!isAutoTransfer)
                return;

            var playerBalance = await _getPlayerBalanceService
                .GetAsync(new GetBalanceQuery(command.PlayerCode), cancellationToken)
                .ConfigureAwait(false);

            var fundOutAmount = (int)Math.Floor(playerBalance.Main);

            if (fundOutAmount > 0)
            {
                await _fundOutService
                    .ExecuteAsync(new FundOutGameBalanceCommand(
                        command.GpCode,
                        command.PlayerCode,
                        (int)Math.Floor(playerBalance.Main)), cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}