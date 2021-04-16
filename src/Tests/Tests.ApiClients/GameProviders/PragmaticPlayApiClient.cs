using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Shared.Game.Models.PragmaticPlay.Incoming;

namespace TGS.Tests.ApiClients.GameProviders
{
    public interface IPragmaticPlayApiClient
    {
        ValueTask<AuthorizePlayerResponse> AuthorizePlayerAsync(AuthorizePlayerRequest request);

        ValueTask<GetBalanceResponse> GetBalanceAsync(GetBalanceRequest request);

        ValueTask<PlaceBetResponse> PlaceBetAsync(PlaceBetRequest request);

        ValueTask<ResultResponse> ResultAsync(ResultRequest request);

        ValueTask<EndRoundResponse> EndRoundAsync(EndRoundRequest request);

        ValueTask<RefundResponse> RefundAsync(RefundRequest request);

        ValueTask<JackpotWinResponse> WinJackpotAsync(JackpotWinRequest request);

        ValueTask<BonusWinResponse> WinBonusAsync(BonusWinRequest request);

        ValueTask<PromoWinResponse> WinPromoAsync(PromoWinRequest request);
    }

    public sealed class PragmaticPlayApiClient : BaseApiClient, IPragmaticPlayApiClient
    {
        private static readonly string RoutePrefix = "api/pp";

        public PragmaticPlayApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        async ValueTask<AuthorizePlayerResponse> IPragmaticPlayApiClient.
            AuthorizePlayerAsync(AuthorizePlayerRequest request)
        {
            var parameters = ConvertToKeyValue(request);
            return await ExecuteFormRequestAsync<AuthorizePlayerResponse>($"{RoutePrefix}/authenticate", parameters)
                .ConfigureAwait(false);
        }

        async ValueTask<GetBalanceResponse> IPragmaticPlayApiClient.GetBalanceAsync(GetBalanceRequest request)
        {
            var parameters = ConvertToKeyValue(request);
            return await ExecuteFormRequestAsync<GetBalanceResponse>($"{RoutePrefix}/balance", parameters)
                .ConfigureAwait(false);
        }

        async ValueTask<PlaceBetResponse> IPragmaticPlayApiClient.PlaceBetAsync(PlaceBetRequest request)
        {
            var parameters = ConvertToKeyValue(request);
            return await ExecuteFormRequestAsync<PlaceBetResponse>($"{RoutePrefix}/bet", parameters)
                .ConfigureAwait(false);
        }

        async ValueTask<RefundResponse> IPragmaticPlayApiClient.RefundAsync(RefundRequest request)
        {
            var parameters = ConvertToKeyValue(request);
            return await ExecuteFormRequestAsync<RefundResponse>($"{RoutePrefix}/refund", parameters)
                .ConfigureAwait(false);
        }

        async ValueTask<ResultResponse> IPragmaticPlayApiClient.ResultAsync(ResultRequest request)
        {
            var parameters = ConvertToKeyValue(request);
            return await ExecuteFormRequestAsync<ResultResponse>($"{RoutePrefix}/result", parameters)
                .ConfigureAwait(false);
        }

        async ValueTask<EndRoundResponse> IPragmaticPlayApiClient.EndRoundAsync(EndRoundRequest request)
        {
            var parameters = ConvertToKeyValue(request);
            return await ExecuteFormRequestAsync<EndRoundResponse>($"{RoutePrefix}/endround", parameters)
                .ConfigureAwait(false);
        }

        async ValueTask<JackpotWinResponse> IPragmaticPlayApiClient.WinJackpotAsync(JackpotWinRequest request)
        {
            var parameters = ConvertToKeyValue(request);
            return await ExecuteFormRequestAsync<JackpotWinResponse>($"{RoutePrefix}/jackpotwin", parameters)
                .ConfigureAwait(false);
        }

        async ValueTask<BonusWinResponse> IPragmaticPlayApiClient.WinBonusAsync(BonusWinRequest request)
        {
            var parameters = ConvertToKeyValue(request);
            return await ExecuteFormRequestAsync<BonusWinResponse>($"{RoutePrefix}/bonuswin", parameters)
                .ConfigureAwait(false);
        }

        async ValueTask<PromoWinResponse> IPragmaticPlayApiClient.WinPromoAsync(PromoWinRequest request)
        {
            var parameters = ConvertToKeyValue(request);
            return await ExecuteFormRequestAsync<PromoWinResponse>($"{RoutePrefix}/promowin", parameters)
                .ConfigureAwait(false);
        }

        private Dictionary<string, string> ConvertToKeyValue(object request)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var value = Json.SerializeToString(request, request.GetType(), options);
            var items = Json.DeserializeFromString<Dictionary<string, object>>(value, options);
            return items.ToDictionary(p => p.Key, p => p.Value?.ToString() ?? string.Empty);
        }
    }
}