using System;
using System.Collections.Generic;
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
    public class PlayerBonus : BaseRepositoryOperations<ITgsRepository>, IPlayerBonus
    {
        private readonly ITgsRepository _repository;

        public PlayerBonus(ITgsRepository repository) : base(repository)
        {
            _repository = repository;
        }

        ITgsRepository IPlayerBonus.Repository => _repository;

        public async Task SentPromotionBonus(IEnumerable<PromotionPlayerBonus> playerBonus,
            TransactionType transactionType)
        {
            var playerCodes = playerBonus.SelectMany(p => p.PlayerBonus).Select(p => p.PlayerCode);
            var players = await _repository.Players
                .Where(x => playerCodes.Contains(x.PlayerCode))
                .Select(x => new {x.BrandId, x.PlayerCode, x.Wallets, x.DefaultWalletId}).ToArrayAsync();

            var transactions = playerBonus.SelectMany(player =>
            {
                return player.PlayerBonus.Join(players, pb => pb.PlayerCode, p => p.PlayerCode, (pb, p) => new {pb, p})
                    .Select(x =>
                    {
                        var wallet = x.p.Wallets.First(w => w.Id == x.p.DefaultWalletId);
                        var bonusAmount = x.pb.BonusAmount;
                        wallet.Balance += bonusAmount;
                        wallet.UpdatedOn = DateTimeOffset.Now;
                        var transactionId = GuidExtensions.NewSequentialGuid();
                        return (
                            new Transaction
                            {
                                Id = transactionId,
                                WalletId = x.p.DefaultWalletId,
                                Wallet = wallet,
                                BrandId = x.p.BrandId,
                                Amount = bonusAmount,
                                Balance = wallet.Balance,
                                Type = transactionType,
                                RecordedOn = DateTimeOffset.Now
                            },
                            new TransactionPromotion
                            {
                                TransactionId = transactionId,
                                PromotionId = player.PromotionId,
                                PromotionBonusId = player.PromotionBonusId,
                                PromotionType = player.PromotionType
                            });
                    }).ToArray();
            }).ToArray();

            var trans = transactions.Select(x => x.Item1).ToArray();
            var transactionPromotions = transactions.Select(x => x.Item2).ToArray();

            await CreateEntitiesAsync(trans);

            await CreateEntitiesAsync(transactionPromotions);
        }

        async Task IPlayerBonus.SentBonus(IEnumerable<PlayerRebates> playerBonus,
            TransactionType transactionType)
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
                    return new Transaction
                    {
                        Id = transactionId,
                        WalletId = x.player.DefaultWalletId,
                        Wallet = wallet,
                        BrandId = x.player.BrandId,
                        Amount = bonusAmount,
                        Balance = wallet.Balance,
                        Type = transactionType,
                        RecordedOn = DateTimeOffset.Now
                    };
                }).ToArray();

            await CreateEntitiesAsync(transactions);
        }
    }
}