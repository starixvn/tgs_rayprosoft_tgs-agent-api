using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Enums;
using TGS.Services.Promotion;
using TGS.Services.Promotion.Models.Promotion;
using PromotionImage = TGS.Core.DataServices.Tgs.Models.PromotionImage;
using PromotionType = TGS.Core.Models.Settings.PromotionType;

namespace TGS.Promotion.Repositories
{
    public class PromotionRepository : BaseRepositoryOperations<ITgsRepository>, IPromotionRepository
    {
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly ITgsRepository _repository;

        public PromotionRepository(ITgsRepository tgsRepository,
            IJsonSerializationProvider jsonSerializationProvider) : base(
            tgsRepository)
        {
            _repository = tgsRepository;
            _jsonSerializationProvider = jsonSerializationProvider;
        }

        public Task ChangePromotionStatusAsync(Core.DataServices.Tgs.Models.Promotion promotion) =>
            UpdateEntityAsync(promotion);

        public async ValueTask<Guid> CreatePromotionAsync(CreatePromotionRequest request,
            PromotionType promotionTypeModel)
        {
            var promotionId = GuidExtensions.NewSequentialGuid();
            var promotion = new Core.DataServices.Tgs.Models.Promotion
            {
                Id = promotionId,
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

            return promotionId;
        }

        public Task<bool> ExistsAsync(
            Expression<Func<Core.DataServices.Tgs.Models.Promotion, bool>> predicate) => base.ExistsAsync(predicate);

        public Task<Core.DataServices.Tgs.Models.Promotion> GetPromotionAsync(
            Expression<Func<Core.DataServices.Tgs.Models.Promotion, bool>> predicate,
            Func<IQueryable<Core.DataServices.Tgs.Models.Promotion>,
                IQueryable<Core.DataServices.Tgs.Models.Promotion>> preQuery) =>
            GetEntityFirstOrDefaultAsync(predicate, preQuery);

        public async Task<Core.DataServices.Tgs.Models.Promotion> GetPromotionByIdAsync(Guid promotionId)
            => await _repository.Promotions.FindAsync(promotionId).ConfigureAwait(false);

        public IQueryable<Core.DataServices.Tgs.Models.Promotion> GetPromotionQueries(
            Func<IQueryable<Core.DataServices.Tgs.Models.Promotion>,
                IQueryable<Core.DataServices.Tgs.Models.Promotion>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        public async Task UpdatePromotionAsync(Core.DataServices.Tgs.Models.Promotion promotion,
            UpdatePromotionRequest request)
        {
            var vips = await _repository.VipLevels.AsQueryable()
                .Where(x => x.BrandId == promotion.BrandId && request.VipLevels.Contains(x.Level)).ToArrayAsync();

            await using var tran = await _repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            await DeleteEntitiesAsync(promotion.PromotionGameProviders);
            await DeleteEntitiesAsync(promotion.PromotionVipLevels);
            await DeleteEntitiesAsync(promotion.PromotionImages);

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
                    .Select(id => new PromotionGameProvider {GameProviderId = id, PromotionId = promotion.Id})
                    .ToHashSet();
            promotion.PromotionVipLevels =
                vips.Select(x => new PromotionVipLevel {VipLevelId = x.Id, Level = x.Level, PromotionId = promotion.Id})
                    .ToHashSet();
            promotion.UpdatedByUsername = request.EditorUsername;
            promotion.UpdatedOn = DateTimeOffset.Now;
            promotion.Subject = request.Subject;
            promotion.SubSubject = request.SubSubject;
            promotion.Content = request.Content;
            promotion.Terms = request.Terms;
            promotion.PromotionImages = request.Images.Select(x => new PromotionImage
            {
                Url = x.Url, PlatformType = x.PlatformType, PromotionId = promotion.Id
            }).ToHashSet();
            promotion.Localization = ToJsonString(request.Localization);

            await UpdateEntityAsync(promotion);
            await tran.CommitAsync();
        }

        public async ValueTask<bool> UpdatePromotionAmountAsync(Guid promotionId, decimal amount)
        {
            const string sql = "dbo.promotion_update_amount_used";
            return await _repository
                .SqlQuerySingleAsync<bool>(
                    sql,
                    parameters: new Dictionary<string, object> {{"_promotion_id", promotionId}, {"_amount", amount}})
                .ConfigureAwait(false);
        }

        public async ValueTask<bool> RevertPromotionAmountAsync(Guid promotionId, decimal amount)
        {
            const string sql = "dbo.promotion_revert_amount_used";
            return await _repository
                .SqlQuerySingleAsync<bool>(
                    sql,
                    parameters: new Dictionary<string, object> {{"_promotion_id", promotionId}, {"_amount", amount}})
                .ConfigureAwait(false);
        }

        public Task UpdatePromotionsAsync(Core.DataServices.Tgs.Models.Promotion[] promotions) =>
            UpdateEntitiesAsync(promotions);

        private HashSet<PromotionGameProvider> GeneratePromotionGameProviders(
            Core.DataServices.Tgs.Models.Promotion promotion,
            CreatePromotionRequest request)
        {
            var promotionGameProviders = _repository.GameProviders.AsQueryable()
                .Where(x => request.GameProviderIds.Contains(x.Id)).Select(x =>
                    new PromotionGameProvider {PromotionId = promotion.Id, GameProviderId = x.Id});
            return new(promotionGameProviders);
        }

        private HashSet<PromotionVipLevel> GeneratePromotionVipLevels(
            Core.DataServices.Tgs.Models.Promotion promotion,
            CreatePromotionRequest request)
        {
            var promotionVipLevels = _repository.VipLevels.AsQueryable()
                .Where(x => x.BrandId == promotion.BrandId && request.VipLevels.Contains(x.Level)).Select(x =>
                    new PromotionVipLevel {PromotionId = promotion.Id, VipLevelId = x.Id, Level = x.Level});
            return new(promotionVipLevels);
        }

        private static string ToJsonString(JsonDocument jsonDocument)
        {
            using var stream = new MemoryStream();
            var writer = new Utf8JsonWriter(stream, new() {Indented = true});
            jsonDocument.WriteTo(writer);
            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}