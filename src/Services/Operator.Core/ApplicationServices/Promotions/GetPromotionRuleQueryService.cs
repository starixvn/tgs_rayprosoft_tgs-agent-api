using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Promotion.Clients;
using Tgs.ServiceUtility;

namespace TGS.Services.Operator.ApplicationServices.Promotions;

internal class GetPromotionRuleQueryService
    : IGetService<GetPromotionRuleQuery, JsonDocument?>
{
    private readonly IPromotionServiceClient _promotionServiceClient;

    public GetPromotionRuleQueryService(
        IPromotionServiceClient promotionServiceClient)
    {
        _promotionServiceClient = promotionServiceClient ?? throw new ArgumentNullException(nameof(promotionServiceClient));
    }

    public async ValueTask<JsonDocument?> GetAsync(
        GetPromotionRuleQuery query,
        CancellationToken cancellationToken = default)
    {
        var rule = await _promotionServiceClient
            .GetPromotionRuleAsync<PromotionRule>(query.PromotionId, cancellationToken)
            .ConfigureAwait(false);

        return rule?.CompaignDetail;
    }

    internal class PromotionRule
    {
        public JsonDocument CompaignDetail { get; init; } = default!;
    }
}
