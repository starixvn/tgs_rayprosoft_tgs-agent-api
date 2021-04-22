using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Bonus;
using TGS.Core.Models.Enums;

namespace Bonus.Repositories
{
    public class SentBonus : BaseRepositoryOperations<ITgsRepository>, ISentBonus
    {
        private readonly ITgsRepository _repository;

        public SentBonus(ITgsRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task Sent(PlayerBonus[] playerBonus, TransactionType transactionType, bool isPromotion)
        {
            var players = await _repository.Players
                .Where(x => playerBonus.Select(p => p.PlayerCode).Contains(x.PlayerCode))
                .Select(x => new {x.BrandId, x.PlayerCode, x.Wallets, x.DefaultWalletId}).ToArrayAsync();

            var transactions = players.Join(playerBonus, p => p.PlayerCode, pb => pb.PlayerCode,
                    (player, playerBonus) => new {player, playerBonus})
                .Select(x =>
                {
                    var wallet = x.player.Wallets.First(w => w.Id == x.player.DefaultWalletId);
                    var bonusAmount = playerBonus.First(p => x.player.PlayerCode == p.PlayerCode).BonusAmount;
                    wallet.Balance += bonusAmount;
                    wallet.UpdatedOn = DateTimeOffset.Now;
                    var transactionId = GuidExtensions.NewSequentialGuid();
                    return (
                        new Transaction
                        {
                            Id = transactionId,
                            WalletId = x.player.DefaultWalletId,
                            Wallet = wallet,
                            BrandId = x.player.BrandId,
                            Amount = bonusAmount,
                            Balance = wallet.Balance,
                            Type = transactionType,
                            RecordedOn = DateTimeOffset.Now
                        },
                        isPromotion
                            ? null
                            : new TransactionPromotion
                            {
                                TransactionId = transactionId,
                                PromotionId = GuidExtensions.NewSequentialGuid(),
                                PromotionType = x.playerBonus.PromotionType
                            });
                }).ToArray();

            await using var transaction = await _repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await CreateEntitiesAsync(transactions.Select(x => x.Item1));

            if (isPromotion)
            {
                await CreateEntitiesAsync(transactions.Select(x => x.Item2));
            }

            await transaction.CommitAsync();
        }
    }
}