using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Operator.Services.Wallet;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.GameBalance
{
    internal class FundInBeforeEnterGameCommandService : ICommandService<FundInBeforeEnterGameCommand>
    {
        private readonly ICommandService<SubmitGameBalanceFundInCommand> _fundInService;
        private readonly IPlayerRepository _playerRepository;
        private readonly ICommandService<FundOutAllGameBalanceCommand> _fundOutService;
        private readonly IGetService<GetBalanceQuery, BalanceInfo> _getPlayerBalanceService;

        public FundInBeforeEnterGameCommandService(
            IPlayerRepository playerRepository,
            ICommandService<FundOutAllGameBalanceCommand> fundOutService,
            IGetService<GetBalanceQuery, BalanceInfo> getPlayerBalanceService,
            ICommandService<SubmitGameBalanceFundInCommand> fundInService)
        {
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _fundOutService = fundOutService ?? throw new ArgumentNullException(nameof(fundOutService));
            _getPlayerBalanceService = getPlayerBalanceService ?? throw new ArgumentNullException(nameof(getPlayerBalanceService));
            _fundInService = fundInService ?? throw new ArgumentNullException(nameof(fundInService));
        }

        public async ValueTask ExecuteAsync(FundInBeforeEnterGameCommand command, CancellationToken cancellationToken = default)
        {
            var isAutoTransfer = await _playerRepository
                .GetPlayerAutoTransferFlagAsync(command.PlayerCode)
                .ConfigureAwait(false);

            if (!isAutoTransfer)
                return;

            await _fundOutService
                .ExecuteAsync(new FundOutAllGameBalanceCommand(
                    command.BrandId,
                    command.PlayerCode,
                    command.GpCode))
                .ConfigureAwait(false);

            var playerBalance = await _getPlayerBalanceService
                .GetAsync(new GetBalanceQuery(command.PlayerCode))
                .ConfigureAwait(false);

            await _fundInService
                .ExecuteAsync(new SubmitGameBalanceFundInCommand(
                    command.GpCode,
                    command.PlayerCode,
                    (int)Math.Floor(playerBalance.Main)))
                .ConfigureAwait(false);
        }
    }
}
