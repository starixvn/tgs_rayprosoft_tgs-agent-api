using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Services.Report.Application.Models.PlayerRebate;

namespace TGS.Services.Report.Infrastructure.DataService
{
    public interface ITgsTransactionService
    {
        ITgsRepository Repository { get; }

        IQueryable<Core.DataServices.Tgs.Models.Transaction> GetTransactionQueries(
            Func<IQueryable<Core.DataServices.Tgs.Models.Transaction>,
                IQueryable<Core.DataServices.Tgs.Models.Transaction>> preQuery = null,
            bool tracking = false);

        Task CreateTransactionsAsync(PlayerRebateModel[] models);
    }

    public class TgsTransactionService : BaseRepositoryOperations<ITgsRepository>, ITgsTransactionService
    {
        public readonly ITgsRepository _repository;

        public TgsTransactionService(ITgsRepository repository) : base(repository)
        {
            _repository = repository;
        }

        ITgsRepository ITgsTransactionService.Repository => _repository;

        async Task ITgsTransactionService.CreateTransactionsAsync(PlayerRebateModel[] models)
        {
            var playerCodes = models.Select(x => x.PlayerCode).ToArray();

            var playerRebates = models.GroupBy(x => new {x.BrandId, x.PlayerCode},
                    (x, list) => new {x.PlayerCode, x.BrandId, list})
                .Join(_repository.Players.Include(x => x.Wallets).Where(x => playerCodes.Contains(x.PlayerCode)),
                    pb => pb.PlayerCode, p => p.PlayerCode,
                    (pb, p) => new {pb.PlayerCode, pb.BrandId, playerRebate = pb.list, player = p})
                .Select(x =>
                {
                    var wallet = x.player.Wallets.FirstOrDefault(w => w.Id == x.player.DefaultWalletId);
                    var rebateAmount = x.playerRebate.Sum(p => p.RebateAmount);
                    wallet.Balance += rebateAmount;
                    wallet.UpdatedOn = DateTimeOffset.Now;

                    return new Core.DataServices.Tgs.Models.Transaction
                    {
                        Id = GuidExtensions.NewSequentialGuid(),
                        WalletId = x.player.DefaultWalletId,
                        Wallet = wallet,
                        BrandId = x.BrandId,
                        Amount = rebateAmount,
                        Balance = wallet.Balance,
                        Type = TransactionType.Rebate,
                        RecordedOn = DateTimeOffset.Now
                    };
                }).ToArray();

            await CreateEntitiesAsync(playerRebates);
        }

        IQueryable<Core.DataServices.Tgs.Models.Transaction> ITgsTransactionService.GetTransactionQueries(
            Func<IQueryable<Core.DataServices.Tgs.Models.Transaction>,
                IQueryable<Core.DataServices.Tgs.Models.Transaction>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);
    }
}