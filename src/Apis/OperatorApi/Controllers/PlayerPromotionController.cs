using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Net.Mime;
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

        [HttpGet]
        [Description("Get player's promotions")]
        [Consumes(MediaTypeNames.Application.Json)]
        [TgsResponseWrapper]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async ValueTask<PagingResponse<PromotionModel>> GetPlayerPromotionsAsync(
            [FromQuery] PagingRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var promotionsRequest = new GetPromotionsRequest
            {
                BrandId = brandId,
                PlayerCode = playerTokenData.PlayerCode,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = request.IsDesc ? SortOrder.Descending : SortOrder.Ascending,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };
            var response = await _client.GetPromotionsAsync(promotionsRequest);

            return new() {TotalCount = response.TotalCount, Items = response.Items ?? Array.Empty<PromotionModel>()};
        }
    }
}