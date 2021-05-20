using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;
using TGS.Services.Promotion;
using TGS.Services.Promotion.Models.Promotion;
using PromotionType = TGS.Core.Models.Settings.PromotionType;

namespace TGS.Promotion.Repositories
{
    public class PromotionRepository : BaseRepositoryOperations<ITgsRepository>, IPromotionRepository
    {
        private readonly ITgsRepository _repository;

        public PromotionRepository(ITgsRepository tgsRepository) : base(
            tgsRepository)
        {
            _repository = tgsRepository;
        }

        public Task ChangePromotionStatusAsync(TGS.Core.DataServices.Tgs.Models.Promotion promotion) =>
            UpdateEntityAsync(promotion);

        public async Task CreatePromotionAsync(CreatePromotionRequest request,
            PromotionType promotionTypeModel)
        {
            var promotion = new TGS.Core.DataServices.Tgs.Models.Promotion
            {
                Id = Guid.NewGuid(),
                BrandId = request.BrandId,
                Name = request.Name,
                From = request.From,
                To = request.To,
                Status = SwitchStatus.Enabled,
                AutoSentPlayerBonus = request.AutoSentPlayerBonus,
                PromotionType = request.PromotionType,
                BonusAmountLimit = request.BonusAmountLimit,
                SettlementIntervalType = promotionTypeModel.SettlementIntervalType,
                AmountUsed = 0,
                Unlimited = request.Unlimited,
                GameType = request.GameType,
                Percentage = request.Percentage,
                TotalAmountLimit = request.TotalAmountLimit,
                TurnoverMultiple = request.TurnoverMultiple,
                CreatedByUsername = request.EditorUsername,
                UpdatedByUsername = request.EditorUsername,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            promotion.PromotionGameProviders = GeneratePromotionGameProviders(promotion, request);
            promotion.PromotionVipLevels = GeneratePromotionVipLevels(promotion, request);

            await CreateEntityAsync(promotion);
        }

        public Task<bool> ExistsAsync(
            Expression<Func<TGS.Core.DataServices.Tgs.Models.Promotion, bool>> predicate) => ExistsAsync(predicate);

        public Task<TGS.Core.DataServices.Tgs.Models.Promotion> GetPromotionAsync(
            Expression<Func<TGS.Core.DataServices.Tgs.Models.Promotion, bool>> predicate,
            Func<IQueryable<TGS.Core.DataServices.Tgs.Models.Promotion>,
                IQueryable<TGS.Core.DataServices.Tgs.Models.Promotion>> preQuery) =>
            GetEntityFirstOrDefaultAsync(predicate, preQuery);

        public async Task<TGS.Core.DataServices.Tgs.Models.Promotion> GetPromotionByIdAsync(Guid promotionId)
            => await _repository.Promotions.FindAsync(promotionId).ConfigureAwait(false);

        public IQueryable<TGS.Core.DataServices.Tgs.Models.Promotion> GetPromotionQueries(
                    Func<IQueryable<TGS.Core.DataServices.Tgs.Models.Promotion>,
                IQueryable<TGS.Core.DataServices.Tgs.Models.Promotion>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        public async Task UpdatePromotionAsync(TGS.Core.DataServices.Tgs.Models.Promotion promotion,
            UpdatePromotionRequest request)
        {
            var vips = await _repository.VipLevels.AsQueryable()
                .Where(x => x.BrandId == promotion.BrandId && request.VipLevels.Contains(x.Level)).ToArrayAsync();

            await using var tran = await _repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            await DeleteEntitiesAsync(promotion.PromotionGameProviders);
            await DeleteEntitiesAsync(promotion.PromotionVipLevels);

            promotion.Name = request.Name;
            promotion.From = request.From;
            promotion.To = request.To;
            promotion.AutoSentPlayerBonus = request.AutoSentPlayerBonus;
            promotion.PromotionType = request.PromotionType;
            promotion.BonusAmountLimit = request.BonusAmountLimit;
            promotion.Unlimited = request.Unlimited;
            promotion.GameType = request.GameType;
            promotion.Percentage = request.Percentage;
            promotion.TotalAmountLimit = request.TotalAmountLimit;
            promotion.TurnoverMultiple = request.TurnoverMultiple;
            promotion.PromotionGameProviders =
                request.GameProviderIds
                    .Select(id => new PromotionGameProvider { GameProviderId = id, PromotionId = promotion.Id })
                    .ToHashSet();
            promotion.PromotionVipLevels =
                vips.Select(x => new PromotionVipLevel { VipLevelId = x.Id, Level = x.Level, PromotionId = promotion.Id })
                    .ToHashSet();
            promotion.UpdatedByUsername = request.EditorUsername;
            promotion.UpdatedOn = DateTimeOffset.Now;

            await UpdateEntityAsync(promotion);
            await tran.CommitAsync();
        }

        private HashSet<PromotionGameProvider> GeneratePromotionGameProviders(
            TGS.Core.DataServices.Tgs.Models.Promotion promotion,
            CreatePromotionRequest request)
        {
            var promotionGameProviders = _repository.GameProviders.AsQueryable()
                .Where(x => request.GameProviderIds.Contains(x.Id)).Select(x =>
                    new PromotionGameProvider { PromotionId = promotion.Id, GameProviderId = x.Id });
            return new(promotionGameProviders);
        }

        private HashSet<PromotionVipLevel> GeneratePromotionVipLevels(
            TGS.Core.DataServices.Tgs.Models.Promotion promotion,
            CreatePromotionRequest request)
        {
            var promotionVipLevels = _repository.VipLevels.AsQueryable()
                .Where(x => x.BrandId == promotion.BrandId && request.VipLevels.Contains(x.Level)).Select(x =>
                    new PromotionVipLevel { PromotionId = promotion.Id, VipLevelId = x.Id, Level = x.Level });
            return new(promotionVipLevels);
        }
    }
}
