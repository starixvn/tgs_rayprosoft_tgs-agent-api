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
using TGS.Core.Models.Messages;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionClient _client;

        public PromotionController(IPromotionClient client)
        {
            _client = client;
        }

        [HttpGet]
        [Description("Get promotions")]
        [Consumes(MediaTypeNames.Application.Json)]
        [TgsResponseWrapper]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async ValueTask<PagedResponse<PromotionModel>> GetPromotionsAsync(
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
            return await _client.GetPromotionsAsync(promotionsRequest);
        }

        [HttpPost("{id}/join")]
        [Description("Join a promotion")]
        [Consumes(MediaTypeNames.Application.Json)]
        [TgsResponseWrapper]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async ValueTask<MessageResponse> JoinPromotionAsync(Guid id)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            return await _client.JoinAsync(new(id, playerTokenData.PlayerCode));
        }
    }
}