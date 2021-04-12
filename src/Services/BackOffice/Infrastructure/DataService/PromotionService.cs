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
using TGS.Services.BackOffice.Application.Models.Promotion;
using TGS.Services.BackOffice.Infrastructure.Services;

namespace TGS.Services.BackOffice.Infrastructure.DataService
{
    public interface IPromotionService
    {
        Task<Promotion> GetPromotionAsync(Expression<Func<Promotion, bool>> predicate,
            Func<IQueryable<Promotion>, IQueryable<Promotion>> preQuery = null);

        IQueryable<Promotion> GetPromotionQueries(Func<IQueryable<Promotion>, IQueryable<Promotion>> preQuery = null,
            bool tracking = false);

        Task CreatePromotionAsync(CreatePromotionRequest request);

        Task UpdatePromotionAsync(Promotion promotion, UpdatePromotionRequest request);

        //Task ChangeStatusAsync(Promotion promotion, UpdatePromotionStatusRequest request);
        Task<bool> ExistsAsync(Expression<Func<Promotion, bool>> predicate);
    }

    public class PromotionService : BaseRepositoryOperations<ITgsRepository>, IPromotionService
    {
        private readonly ICredentialGenerator _credentialGenerator;
        private readonly ITgsRepository _repository;

        public PromotionService(ITgsRepository tgsRepository, ICredentialGenerator credentialGenerator) : base(
            tgsRepository)
        {
            _repository = tgsRepository;
            _credentialGenerator = credentialGenerator;
        }

        //Task IPromotionService.ChangeStatusAsync(Promotion promotion, UpdatePromotionStatusRequest request)
        //{
        //    promotion.Status = request.Status;
        //    promotion.UpdatedOn = DateTimeOffset.Now;
        //    promotion.UpdatedByUsername = request.EditorUsername;

        //    return UpdateEntityAsync(promotion);
        //}

        Task<Promotion> IPromotionService.GetPromotionAsync(Expression<Func<Promotion, bool>> predicate,
            Func<IQueryable<Promotion>, IQueryable<Promotion>> preQuery) =>
            GetEntityFirstOrDefaultAsync(predicate, preQuery);

        Task<bool> IPromotionService.ExistsAsync(Expression<Func<Promotion, bool>> predicate) => ExistsAsync(predicate);

        IQueryable<Promotion> IPromotionService.GetPromotionQueries(
            Func<IQueryable<Promotion>, IQueryable<Promotion>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        async Task IPromotionService.UpdatePromotionAsync(Promotion promotion, UpdatePromotionRequest request)
        {
            var vips = await _repository.VipLevels
                .Where(x => x.BrandId == promotion.BrandId && request.VipLevels.Contains(x.Level)).ToArrayAsync();

            await using var tran = await _repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            await DeleteEntitiesAsync(promotion.PromotionGameProviders);
            await DeleteEntitiesAsync(promotion.PromotionVipLevels);

            promotion.Name = request.Name;
            promotion.From = request.From;
            promotion.To = request.To;
            promotion.Status = request.Status;
            promotion.PromotionType = request.PromotionType;
            promotion.PromotionAmount = request.PromotionAmount;
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

            await UpdateEntityAsync(promotion);
            await tran.CommitAsync();
        }

        async Task IPromotionService.CreatePromotionAsync(CreatePromotionRequest request)
        {
            var promotion = new Promotion
            {
                Id = Guid.NewGuid(),
                BrandId = request.BrandId,
                Name = request.Name,
                From = request.From,
                To = request.To,
                Status = request.Status,
                PromotionType = request.PromotionType,
                PromotionAmount = request.PromotionAmount,
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

        private HashSet<PromotionGameProvider> GeneratePromotionGameProviders(Promotion promotion,
            CreatePromotionRequest request)
        {
            var promotionGameProviders = _repository.GameProviders
                .Where(x => request.GameProviderIds.Contains(x.Id)).Select(x =>
                    new PromotionGameProvider {PromotionId = promotion.Id, GameProviderId = x.Id});
            return new(promotionGameProviders);
        }

        private HashSet<PromotionVipLevel> GeneratePromotionVipLevels(Promotion promotion,
            CreatePromotionRequest request)
        {
            var promotionVipLevels = _repository.VipLevels
                .Where(x => x.BrandId == promotion.BrandId && request.VipLevels.Contains(x.Level)).Select(x =>
                    new PromotionVipLevel {PromotionId = promotion.Id, VipLevelId = x.Id, Level = x.Level});
            return new(promotionVipLevels);
        }
    }
}