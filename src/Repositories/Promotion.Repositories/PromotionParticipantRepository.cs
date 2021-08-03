using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.Promotion;
using TGS.Services.Promotion.Models.Promotion;

namespace TGS.Promotion.Repositories
{
    public class PromotionParticipantRepository : BaseRepositoryOperations<ITgsRepository>,
        IPromotionParticipantRepository
    {
        private readonly ITgsRepository _repository;

        public PromotionParticipantRepository(ITgsRepository tgsRepository) : base(
            tgsRepository)
        {
            _repository = tgsRepository;
        }

        Task<bool> IPromotionParticipantRepository.ExistsAsync(
            Expression<Func<PromotionParticipant, bool>> predicate) => ExistsAsync(predicate);

        IQueryable<PromotionParticipant> IPromotionParticipantRepository.GetPromotionParticipantQueries(
            Func<IQueryable<PromotionParticipant>,
                IQueryable<PromotionParticipant>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        async Task IPromotionParticipantRepository.CreatePromotionParticipantAsync(Guid promotionId, string playerCode)
        {
            var promotionParticipant =
                new PromotionParticipant
                {
                    PlayerCode = playerCode,
                    PromotionId = promotionId,
                    CreatedOn = DateTimeOffset.Now
                };

            await CreateEntityAsync(promotionParticipant);
        }

        Task<PromotionParticipant> IPromotionParticipantRepository.GetPromotionParticipantAsync(
            Expression<Func<PromotionParticipant, bool>> predicate,
            Func<IQueryable<PromotionParticipant>,
                IQueryable<PromotionParticipant>> preQuery) =>
            GetEntityFirstOrDefaultAsync(predicate, preQuery);

        private HashSet<PromotionGameProvider> GeneratePromotionGameProviders(
            Core.DataServices.Tgs.Models.Promotion promotion,
            CreatePromotionRequest request)
        {
            var promotionGameProviders = _repository.GameProviders.AsQueryable()
                .Where(x => request.GameProviderIds.Contains(x.Id)).Select(x =>
                    new PromotionGameProvider
                    {
                        PromotionId = promotion.Id,
                        GameProviderId = x.Id
                    });
            return new(promotionGameProviders);
        }

        private HashSet<PromotionVipLevel> GeneratePromotionVipLevels(
            Core.DataServices.Tgs.Models.Promotion promotion,
            CreatePromotionRequest request)
        {
            var promotionVipLevels = _repository.VipLevels.AsQueryable()
                .Where(x => x.BrandId == promotion.BrandId && request.VipLevels.Contains(x.Level)).Select(x =>
                    new PromotionVipLevel
                    {
                        PromotionId = promotion.Id,
                        VipLevelId = x.Id,
                        Level = x.Level
                    });
            return new(promotionVipLevels);
        }
    }
}