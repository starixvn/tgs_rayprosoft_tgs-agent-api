using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.PlayerRebate;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Commands.Player
{
    public class
        CreatePlayerRebatesHandler : Handler<CreatePlayerRebateRequest,
            MessageResponse>
    {
        private readonly IPlayerGameTypeAggregateService _playerGameTypeAggregateService;
        private readonly IPlayerRebateService _playerRebateService;
        private readonly ITgsPlayerService _playerService;
        private readonly ITgsTransactionService _transactionService;

        public CreatePlayerRebatesHandler(ITgsTransactionService transaction,
            IPlayerRebateService playerRebateService,
            ITgsPlayerService player,
            IPlayerGameTypeAggregateService playerGameTypeAggregateService,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _transactionService = transaction;
            _playerService = player;
            _playerRebateService = playerRebateService;
            _playerGameTypeAggregateService = playerGameTypeAggregateService;
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

            var playerRebates = await _playerGameTypeAggregateService
                .GetPlayerGameTypeAggregateQueries(q => q.Where(x => x.ReportDate == reportDate))
                .GroupBy(
                    x => new
                    {
                        x.BrandId,
                        x.PlayerCode,
                        x.AgentId,
                        x.VipCardGameRebatePercentage,
                        x.VipLiveDealerRebatePercentage,
                        x.VipLotteryRebatePercentage,
                        x.VipSlotRebatePercentage,
                        x.VipSportsRebatePercentage,
                        x.VipVirtualSportsRebatePercentage
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
                            list.Where(l => l.GameType == GameType.Sportsbook || l.GameType == GameType.ESports)
                                .Sum(a => a.Turnover),
                        SportsRebateAmount = x.VipSportsRebatePercentage * list.Where(a =>
                                a.GameType == GameType.Sportsbook || a.GameType == GameType.ESports)
                            .Sum(a => a.Turnover) / 100,
                        VirtualSportsTurnover =
                            list.Where(l => l.GameType == GameType.VirtualSports).Sum(a => a.Turnover),
                        VirtualSportsRebateAmount = x.VipVirtualSportsRebatePercentage * list
                            .Where(a => a.GameType == GameType.VirtualSports)
                            .Sum(a => a.Turnover) / 100,
                        LiveDealerTurnover = list.Where(l => l.GameType == GameType.LiveDealer).Sum(a => a.Turnover),
                        LiveDealerRebateAmount = x.VipLiveDealerRebatePercentage * list
                            .Where(a => a.GameType == GameType.LiveDealer)
                            .Sum(a => a.Turnover) / 100,
                        CardGameTurnover = list.Where(l => l.GameType == GameType.CardGame).Sum(a => a.Turnover),
                        CardGameRebateAmount =
                            x.VipCardGameRebatePercentage * list.Where(a => a.GameType == GameType.CardGame)
                                .Sum(a => a.Turnover) / 100,
                        SlotTurnover = list.Where(l =>
                            (l.GameType == GameType.Fishing) | (l.GameType == GameType.Arcade) ||
                            l.GameType == GameType.Slot || l.GameType == GameType.TableGame).Sum(a => a.Turnover),
                        SlotRebateAmount =
                            x.VipSlotRebatePercentage * list.Where(a =>
                                    a.GameType == GameType.Fishing || a.GameType == GameType.Arcade ||
                                    a.GameType == GameType.Slot || a.GameType == GameType.TableGame)
                                .Sum(a => a.Turnover) / 100,
                        LotteryTurnover = list.Where(l => l.GameType == GameType.Lottery).Sum(a => a.Turnover),
                        LotteryRebateAmount =
                            x.VipLotteryRebatePercentage * list.Where(a => a.GameType == GameType.Lottery)
                                .Sum(a => a.Turnover) / 100,
                        RebateAmount = list.Sum(l => l.VipRebateAmount)
                    })
                .ToArrayAsync(cancellationToken);

            if (playerRebates.Length <= 0)
            {
                return new() {Valid = false, Message = "Has no player rebate data."};
            }

            await using var reportTransaction =
                await _playerRebateService.Repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            await using var tgsTransaction =
                await _transactionService.Repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                await _playerRebateService.CreatePlayerRebatesAsync(playerRebates);

                await _transactionService.CreateTransactionsAsync(playerRebates);

                await reportTransaction.CommitAsync(cancellationToken);
                await tgsTransaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await reportTransaction.RollbackAsync(cancellationToken);
                await tgsTransaction.RollbackAsync(cancellationToken);
                return CreateInvalidResponse<MessageResponse>(e.Message);
            }

            return CreateValidResponse<MessageResponse>();
        }
    }
}