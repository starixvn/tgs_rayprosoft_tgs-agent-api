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
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        public async ValueTask<PagingResponse<PromotionModel>> GetPromotionsAsync(
            [FromQuery] PagingRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();

            var promotionsRequest = new GetPromotionsRequest
            {
                BrandId = brandId,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = request.IsDesc ? SortOrder.Descending : SortOrder.Ascending,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll,
                CurrentDate = DateTimeOffset.Now
            };
            var response = await _client.GetPromotionsAsync(promotionsRequest);

            return new()
            {
                TotalCount = response.TotalCount,
                Items = response.Items ?? Array.Empty<PromotionModel>()
            };
        }

        [HttpPost("{id}/join")]
        [Description("Join a promotion")]
        [Consumes(MediaTypeNames.Application.Json)]
        [TgsResponseWrapper]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async ValueTask<JoinPromotionResponse> JoinPromotionAsync(Guid id)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var response = await _client.JoinAsync(new(id, playerTokenData.PlayerCode));

            return new() { Success = response.Valid, ErrorMessage = !response.Valid ? response.Message : string.Empty };
        }
    }
}