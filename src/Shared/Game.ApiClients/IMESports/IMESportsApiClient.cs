// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Json.JsonConverters;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Shared.Game.Models.IMESports.Outgoing;

namespace TGS.Shared.Game.ApiClients.IMESports
{
    public interface IIMESportsApiClient
    {
        ValueTask<NewLaunchGameResponse> NewLaunchGameAsync(Uri apiUrl, PlatformType platformType, NewLaunchGameRequest request);

        ValueTask<RegisterPlayerResponse> RegisterPlayerAsync(Uri apiUrl, RegisterPlayerRequest request);

        ValueTask<GetRoundHistoryResponse> GetRoundHistoryAsync(Uri apiUrl, GetRoundHistoryRequest request);
    }

    public sealed class IMESportsApiClient : BaseApiClient, IIMESportsApiClient
    {
        public IMESportsApiClient(HttpClient client, IApiClientLogger logger, IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        ValueTask<NewLaunchGameResponse> IIMESportsApiClient.NewLaunchGameAsync(Uri apiUrl, PlatformType platformType, NewLaunchGameRequest request)
        {
            var prefix = $"{Consts.GameProvider.IMESports}.NewLaunchGame";
            var path = platformType == PlatformType.Desktop ? "NewLaunchGame" : "NewLaunchMobileGame";
            return ExecuteJsonRequestAsync<NewLaunchGameRequest, NewLaunchGameResponse>(HttpMethod.Post, apiUrl,
                $"/Game/{path}", request, logPrefix: prefix);
        }

        ValueTask<RegisterPlayerResponse> IIMESportsApiClient.RegisterPlayerAsync(Uri apiUrl, RegisterPlayerRequest request)
        {
            var prefix = $"{Consts.GameProvider.IMESports}.RegisterPlayer";
            return ExecuteJsonRequestAsync<RegisterPlayerRequest, RegisterPlayerResponse>(
                HttpMethod.Post, apiUrl,
                "/Player/Register",
                request, logPrefix: prefix);
        }

        ValueTask<GetRoundHistoryResponse> IIMESportsApiClient.GetRoundHistoryAsync(Uri apiUrl,
            GetRoundHistoryRequest request)
        {
            var options = new JsonSerializerOptions {Converters = {new NumberToStringConverter()}};

            var prefix = $"{Consts.GameProvider.IMESports}.RoundHistory";
            return ExecuteJsonRequestAsync<GetRoundHistoryRequest, GetRoundHistoryResponse>(
                HttpMethod.Post, apiUrl, "/Report/GetBetLog",
                request, logPrefix: prefix, options: options);
        }
    }
}