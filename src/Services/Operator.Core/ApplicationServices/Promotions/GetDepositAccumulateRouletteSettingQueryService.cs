using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Promotion.Clients;
using Tgs.ServiceUtility;

namespace TGS.Services.Operator.ApplicationServices.Promotions;

internal class GetDepositAccumulateRouletteSettingQueryService
    : IGetService<GetDepositAccumulateRouletteSettingQuery, DepositAccumulateRouletteSetting?>
{
    private readonly IPromotionServiceClient _promotionServiceClient;

    public GetDepositAccumulateRouletteSettingQueryService(
        IPromotionServiceClient promotionServiceClient)
    {
        _promotionServiceClient = promotionServiceClient ?? throw new ArgumentNullException(nameof(promotionServiceClient));
    }

    public async ValueTask<DepositAccumulateRouletteSetting?> GetAsync(
        GetDepositAccumulateRouletteSettingQuery query,
        CancellationToken cancellationToken = default)
    {
        var mainInfo = await _promotionServiceClient
            .GetPromotionMainInfoAsync(query.PromotionId, cancellationToken)
            .ConfigureAwait(false);

        if (mainInfo is null)
            return null;

        if (mainInfo.PromotionType != "DepositAccumulateRoulette")
            throw new InvalidOperationException(
                $"Promotion with id {query.PromotionId} is not a DepositAccumulateRoulette promotion");

        var rule = await _promotionServiceClient
            .GetPromotionRuleAsync<PromotionRule>(query.PromotionId, cancellationToken)
            .ConfigureAwait(false);

        if (rule is null)
            return null;

        if (!rule.CompaignDetail.RootElement.TryGetProperty("rouletteItems", out var items))
            return null;

        var rouletteItems = items.EnumerateArray()
            .Select(elem =>
            {
                var id = Guid.Parse(elem.GetProperty("id").GetString()!);
                var title = elem.GetProperty("title").GetString()!;
                return new DepositAccumulateRouletteItem(id, title);
            });

        return new DepositAccumulateRouletteSetting(rouletteItems);
    }

    internal class PromotionRule
    {
        public JsonDocument CompaignDetail { get; init; } = default!;
    }
}
