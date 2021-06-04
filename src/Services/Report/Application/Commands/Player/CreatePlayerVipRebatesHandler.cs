using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.PlayerRebate;
using TGS.Services.Report.Infrastructure.DataService;
using TGS.Services.WalletService;

namespace TGS.Services.Report.Application.Commands.Player
{
    public class
        CreatePlayerVipRebatesHandler : Handler<CreatePlayerRebateRequest,
            MessageResponse>
    {
        private readonly ITgsBrandService _brandService;
        private readonly IPlayerGameAggregateService _playerGameAggregateService;
        private readonly IPlayerRebateService _playerRebateService;
        private readonly ITgsPlayerService _playerService;
        private readonly IWalletRepository _walletRepository;

        public CreatePlayerVipRebatesHandler(ITgsBrandService brandService,
            IPlayerRebateService playerRebateService,
            IPlayerGameAggregateService playerGameAggregateService,
            IWalletRepository walletRepository,
            ITgsPlayerService playerService,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _brandService = brandService;
            _playerRebateService = playerRebateService;
            _playerGameAggregateService = playerGameAggregateService;
            _walletRepository = walletRepository;
            _playerService = playerService;
        }

        public override async Task<MessageResponse> Handle(CreatePlayerRebateRequest request,
            CancellationToken cancellationToken)
        {
            var previousDay = DateTimeOffset.Now.ToOffset(new(8, 0, 0)).AddDays(-1);
            var reportDate = new DateTimeOffset(previousDay.Year, previousDay.Month, previousDay.Day, 0, 0, 0,
                previousDay.Offset);

            if (request.Date.IsNotNullOrEmpty() &&
                DateTimeOffset.TryParse($"{request.Date} 00:00 +08:00", out var result))
            {
                reportDate = new(result.Year, result.Month, result.Day, 0, 0, 0,
                    result.Offset);
            }

            if (await _playerRebateService.ExistsAsync(x => x.ReportDate == reportDate))
            {
                return CreateInvalidResponse<CreatePlayerRebateResponse>(
                    $"The rebates of {reportDate} have generated.");
            }

            var playerRebates = await _playerGameAggregateService
                .GetPlayerGameAggregateQueries(q => q.Where(x => x.ReportDate == reportDate))
                .GroupJoin(_playerGameAggregateService.Repository.BrandGames,
                    aggregate => new {aggregate.BrandId, aggregate.GameId},
                    brandGame => new {brandGame.BrandId, brandGame.GameId},
                    (aggregate, brandGame) => new {aggregate, brandGame})
                .SelectMany(x => x.brandGame.DefaultIfEmpty(),
                    (x, brandGame) => new {x.aggregate, brandGame})
                .Where(x => x.brandGame == null || x.brandGame.Rebate)
                .GroupBy(
                    x => new
                    {
                        x.aggregate.BrandId,
                        x.aggregate.PlayerCode,
                        x.aggregate.AgentId,
                        x.aggregate.VipCardGameRebatePercentage,
                        x.aggregate.VipLiveDealerRebatePercentage,
                        x.aggregate.VipLotteryRebatePercentage,
                        x.aggregate.VipSlotRebatePercentage,
                        x.aggregate.VipSportsRebatePercentage,
                        x.aggregate.VipVirtualSportsRebatePercentage
                    },
                    (x, list) => new PlayerRebateModel
                    {
                        BrandId = x.BrandId,
                        PlayerCode = x.PlayerCode,
                        AgentId = x.AgentId,
                        ReportDate = reportDate,
                        VipCardGameRebatePercentage = x.VipCardGameRebatePercentage,
                        VipLiveDealerRebatePercentage = x.VipLiveDealerRebatePercentage,
                        VipLotteryRebatePercentage = x.VipLotteryRebatePercentage,
                        VipSlotRebatePercentage = x.VipSlotRebatePercentage,
                        VipSportsRebatePercentage = x.VipSportsRebatePercentage,
                        VipVirtualSportsRebatePercentage = x.VipVirtualSportsRebatePercentage,
                        SportsTurnover =
                            list.Where(l => Consts.SportsGroup.Contains(l.aggregate.GameType))
                                .Sum(x => x.aggregate.Turnover),
                        SportsRebateAmount = x.VipSportsRebatePercentage * list.Where(l =>
                                Consts.SportsGroup.Contains(l.aggregate.GameType))
                            .Sum(x => x.aggregate.Turnover) / 100M,
                        VirtualSportsTurnover =
                            list.Where(l => l.aggregate.GameType == GameType.VirtualSports)
                                .Sum(a => a.aggregate.Turnover),
                        VirtualSportsRebateAmount =
                            x.VipVirtualSportsRebatePercentage *
                            list.Where(l => l.aggregate.GameType == GameType.VirtualSports)
                                .Sum(a => a.aggregate.Turnover) / 100M,
                        LiveDealerTurnover =
                            list.Where(l => l.aggregate.GameType == GameType.LiveDealer).Sum(a => a.aggregate.Turnover),
                        LiveDealerRebateAmount =
                            x.VipLiveDealerRebatePercentage *
                            list.Where(l => l.aggregate.GameType == GameType.LiveDealer)
                                .Sum(a => a.aggregate.Turnover) / 100M,
                        CardGameTurnover =
                            list.Where(l => l.aggregate.GameType == GameType.CardGame).Sum(a => a.aggregate.Turnover),
                        CardGameRebateAmount =
                            x.VipCardGameRebatePercentage *
                            list.Where(l => l.aggregate.GameType == GameType.CardGame).Sum(a => a.aggregate.Turnover) /
                            100M,
                        SlotTurnover =
                            list.Where(l => Consts.SlotGroup.Contains(l.aggregate.GameType))
                                .Sum(a => a.aggregate.Turnover),
                        SlotRebateAmount =
                            x.VipSlotRebatePercentage * list.Where(l =>
                                Consts.SlotGroup.Contains(l.aggregate.GameType)).Sum(a => a.aggregate.Turnover) /
                            100M,
                        LotteryTurnover = list.Where(a => a.aggregate.GameType == GameType.Lottery)
                            .Sum(a => a.aggregate.Turnover),
                        LotteryRebateAmount =
                            x.VipLotteryRebatePercentage * list.Where(a => a.aggregate.GameType == GameType.Lottery)
                                .Sum(a => a.aggregate.Turnover) / 100M,
                        RebateAmount = list.Sum(l => l.aggregate.VipRebateAmount)
                    })
                .ToArrayAsync(cancellationToken);

            if (playerRebates.Length <= 0)
            {
                return new() {Valid = false, Message = "Has no player rebate data."};
            }

            var brandIds = playerRebates.Select(x => x.BrandId);
            var brands = await _brandService.GetBrandQueries().Where(x => brandIds.Contains(x.Id))
                .ToArrayAsync(cancellationToken);

            var playerBonuses = playerRebates
                .Join(brands, p => p.BrandId, b => b.Id, (player, brand) => new {player, brand})
                .Where(x => x.brand.AutoSentPlayerBonus).Select(x => x.player).ToArray();

            var playerBonusesNotSend = playerRebates
                .Join(brands, p => p.BrandId, b => b.Id, (player, brand) => new {player, brand})
                .Where(x => !x.brand.AutoSentPlayerBonus).Select(x => x.player).ToArray();

            var players = await _playerService.GetPlayerQueries()
                .Where(x => playerBonuses.Select(p => p.PlayerCode).Contains(x.PlayerCode))
                .Select(x => new {x.BrandId, x.PlayerCode, x.Wallets, x.DefaultWalletId})
                .ToArrayAsync(cancellationToken);

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            await _playerRebateService.CreatePlayerRebatesAsync(playerBonuses, DateTimeOffset.Now);

            await _playerRebateService.CreatePlayerRebatesAsync(playerBonusesNotSend, null);

            foreach (var playerBonus in playerBonuses.Join(players, pb => pb.PlayerCode, p => p.PlayerCode,
                (pb, p) => new {pb, p}))
            {
                await _walletRepository.ExchangeAsync(playerBonus.pb.BrandId, playerBonus.p.DefaultWalletId,
                    playerBonus.pb.PlayerCode, TransactionTypes.Rebate,
                    playerBonus.pb.RebateAmount);
            }

            scope.Complete();
            return CreateValidResponse<MessageResponse>();
        }
    }
}