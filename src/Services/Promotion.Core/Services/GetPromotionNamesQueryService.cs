using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TGS.Services.Promotion.Models.Promotion;
using Tgs.ServiceUtility;

namespace TGS.Services.Promotion.Services
{
    internal class GetPromotionNamesQueryService : IQueryService<GetPromotionNamesQuery, PromotionNameModel>
    {
        private readonly IPromotionRepository _promotionRepository;

        public GetPromotionNamesQueryService(
            IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository ?? throw new ArgumentNullException(nameof(promotionRepository));
        }

        public IAsyncEnumerable<PromotionNameModel> QueryAsync(
            GetPromotionNamesQuery query, CancellationToken cancellationToken = default)
            => _promotionRepository.GetPromotionQueries().Where(x => query.PromotionIds.Contains(x.Id))
                .Select(x => new PromotionNameModel
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToAsyncEnumerable();
    }
}