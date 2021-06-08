using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.PlayerBucket;
using TGS.Services.PlayerBucket.Models;
using TGS.Services.Promotion.Models.PromotionDispatch;
using TGS.Services.TgsTransaction.Core;
using TGS.Services.WalletService;

namespace TGS.Services.Promotion.Handlers
{
    public class DispatchPromotionBonusHandler : Handler<DispatchPromotionBonusRequest, MessageResponse>
    {
        private readonly IPromotionBonusRepository _promotionBonusService;
        private readonly IPlayerBucketRepository _service;
        private readonly ITgsTransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;

        public DispatchPromotionBonusHandler(IPlayerBucketRepository service,
            IPromotionBonusRepository promotionBonusService,
            IWalletRepository walletRepository,
            ITgsTransactionRepository transactionRepository,
            IHttpContextAccessor accessor) : base(
            accessor)
        {
            _service = service;
            _walletRepository = walletRepository;
            _promotionBonusService = promotionBonusService;
            _transactionRepository = transactionRepository;
        }

        public override async Task<MessageResponse> Handle(DispatchPromotionBonusRequest request,
            CancellationToken cancellationToken)
        {
            var playerCodes = request.Players.Select(x => x.PlayerCode).ToArray();
            var promotionBonusIds = request.Players.Select(x => x.PromotionBonusId).ToArray();

            var promotionPlayers = await _promotionBonusService.GetPromotionBonusPlayersQueries()
                .Include(x => x.Player)
                .Include(x => x.PromotionBonus)
                .Where(x => promotionBonusIds.Contains(x.PromotionBonusId))
                .Where(x => playerCodes.Contains(x.PlayerCode))
                .Where(x => x.SentOn == null).ToArrayAsync(cancellationToken);

            var players = promotionPlayers.Where(x =>
                    request.Players.Any(p => p.PlayerCode == x.PlayerCode && p.PromotionBonusId == x.PromotionBonusId))
                .ToArray();

            var playerBuckets = players.Select(x => new
                {
                    x.Player.BrandId,
                    x.PlayerCode,
                    x.Player.DefaultWalletId,
                    BucketAmount = x.BonusAmount * x.PromotionBonus.TurnoverMultiple,
                    x.BonusAmount,
                    BucketType = BucketType.Promotion,
                    x.PromotionBonus.PromotionId,
                    x.PromotionBonusId,
                    x.PromotionBonus.PromotionType
                })
                .ToArray();

            foreach (var promotionPlayer in players)
            {
                promotionPlayer.SentOn = DateTimeOffset.Now;
            }

            await _service.CreatePlayerBucketsAsync(playerBuckets.Select(x =>
                    new CreatePlayerBucketRequest(x.BrandId, x.PlayerCode, x.DefaultWalletId, x.BucketType,
                        x.BucketAmount))
                .ToArray()).ConfigureAwait(false);

            await _promotionBonusService.UpdatePromotionBonusPlayersAsync(players);

            var transactionPromotions = new List<TransactionPromotion>();
            foreach (var promotionPlayer in playerBuckets)
            {
                var (transactionId, balance) = await _walletRepository.ExchangeAsync(promotionPlayer.BrandId,
                    promotionPlayer.DefaultWalletId, promotionPlayer.PlayerCode, TransactionTypes.Promotion,
                    promotionPlayer.BonusAmount).ConfigureAwait(false);

                transactionPromotions.Add(new()
                {
                    TransactionId = transactionId,
                    PromotionId = promotionPlayer.PromotionId,
                    PromotionBonusId = promotionPlayer.PromotionBonusId,
                    PromotionType = promotionPlayer.PromotionType
                });
            }

            await _transactionRepository.CreateTransactionPromotionsAsync(transactionPromotions.ToArray());

            return CreateValidResponse<MessageResponse>();
        }
    }
}