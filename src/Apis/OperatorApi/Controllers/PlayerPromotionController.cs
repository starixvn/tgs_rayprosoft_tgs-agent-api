using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Clients.Promotion.Client;
using TGS.Clients.Promotion.Client.Models.Promotion;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class PlayerPromotionController : ControllerBase
    {
        private readonly IPromotionClient _client;

        public PlayerPromotionController(IPromotionClient client)
        {
            _client = client;
        }

        [HttpGet("{language}")]
        [Description("Get player's promotions")]
        [Consumes(MediaTypeNames.Application.Json)]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async ValueTask<PagingResponse<PromotionResponse>> GetPlayerPromotionsAsync(string language,
            [FromQuery] PagingRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var promotionsRequest = new GetValidPromotionsRequest
            {
                BrandId = brandId,
                Language = language,
                PlayerCode = playerTokenData.PlayerCode,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = request.IsDesc ? SortOrder.Descending : SortOrder.Ascending,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };

            var response = await _client.GetValidPromotionsAsync(promotionsRequest);

            return new PagingResponse<PromotionResponse>
            {
                TotalCount = response.TotalCount,
                Items = response.Items.Select(item => new PromotionResponse
                {
                    AmountUsed = item.AmountUsed,
                    AutoSentPlayerBonus = item.AutoSentPlayerBonus,
                    BonusAmountLimit = item.BonusAmountLimit,
                    BrandId = item.BrandId,
                    Content = item.Content,
                    CreatedByUsername = item.CreatedByUsername,
                    CreatedOn = item.CreatedOn,
                    DesktopBanner = item.DesktopBanner,
                    From = item.From,
                    GameProviderIds = item.GameProviderIds,
                    GameType = item.GameType,
                    Id = item.Id,
                    Localization = item.Localization?.ContainsKey(language) == true
                        ? item.Localization[language]
                        : JsonDocument.Parse("{}"),
                    MobileBanner = item.MobileBanner,
                    Name = item.Name,
                    Percentage = item.Percentage,
                    PromotionType = item.PromotionType,
                    Status = item.Status,
                    SubSubject = item.SubSubject,
                    Terms = item.Terms,
                    To = item.To,
                    TotalAmountLimit = item.TotalAmountLimit,
                    TurnoverMultiple = item.TurnoverMultiple,
                    Unlimited = item.Unlimited,
                    UpdatedByUsername = item.UpdatedByUsername,
                    UpdatedOn = item.UpdatedOn,
                    VipLevels = item.VipLevels
                })
            };
        }
    }
}
