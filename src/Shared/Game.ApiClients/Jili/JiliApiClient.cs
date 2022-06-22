using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Shared.Game.Models.Jili.Outgoing;

namespace TGS.Shared.Game.ApiClients.Jili;

public interface IJiliApiClient
{
    ValueTask<JiliGetGameDetailUrlResponse> GetGameDetailUrlAsync(
        Uri apiUrl,
        string agentId,
        string agentKey,
        string language,
        string externalRoundId);

    ValueTask<JiliLoginWithoutRedirectResponse> LoginWithoutRedirectAsync(
                Uri apiUrl,
        string agentId,
        string agentKey,
        JiliLoginWithoutRedirectRequest request);
}

internal class JiliApiClient : BaseApiClient, IJiliApiClient
{
    private const string AlphanumericCharacters = "123456789abcdefghijklmnopqrstuvwxyz";
    private static readonly Random _Random = new Random();

    public JiliApiClient(
        HttpClient client,
        IApiClientLogger logger,
        IJsonSerializationProvider json)
        : base(client, logger, json)
    {
    }

    public async ValueTask<JiliGetGameDetailUrlResponse> GetGameDetailUrlAsync(
        Uri apiUrl,
        string agentId,
        string agentKey,
        string language,
        string externalRoundId)
    {
        var parameters = new Dictionary<string, string>
        {
            ["WagersId"] = externalRoundId,
            ["AgentId"] = agentId
        };

        var keyGen = GetKeyGen(agentId, agentKey);
        var key = GetKey(keyGen, parameters);
        parameters["Key"] = key;
        parameters["Lang"] = language;

        var prefix = $"{Consts.GameProvider.Jili}.GetGameDetailUrl";

        var queryString = string.Join("&", parameters.Select(pair => $"{pair.Key}={pair.Value}"));
        var uriString = $"{apiUrl.AbsoluteUri.TrimEndSlash()}/api1/GetGameDetailUrl?{queryString}";

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uriString)
        {
            Content = new FormUrlEncodedContent(parameters)
        };

        var response = await SendAsync(httpRequestMessage, prefix).ConfigureAwait(false);

        var responseMessage = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<JiliGetGameDetailUrlResponse>(responseMessage);
    }

    public async ValueTask<JiliLoginWithoutRedirectResponse> LoginWithoutRedirectAsync(
        Uri apiUrl,
        string agentId,
        string agentKey,
        JiliLoginWithoutRedirectRequest request)
    {
        var value = JsonSerializer.Serialize(request);
        var parameters = JsonSerializer.Deserialize<Dictionary<string, string>>(value)!;
        parameters["AgentId"] = agentId;

        var keyGen = GetKeyGen(agentId, agentKey);
        var key = GetKey(keyGen, parameters);
        parameters["Key"] = key;

        var prefix = $"{Consts.GameProvider.Jili}.LoginWithoutRedirect";

        var queryString = string.Join("&", parameters.Select(pair => $"{pair.Key}={pair.Value}"));
        var uriString = $"{apiUrl.AbsoluteUri.TrimEndSlash()}/singleWallet/LoginWithoutRedirect?{queryString}";

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uriString);

        var response = await SendAsync(httpRequestMessage, prefix).ConfigureAwait(false);

        var responseMessage = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<JiliLoginWithoutRedirectResponse>(responseMessage);
    }

    private string GenerateString(int length)
    {
        var result = Enumerable
            .Range(1, length)
            .Select(n => AlphanumericCharacters[_Random.Next(0, AlphanumericCharacters.Length)])
            .ToArray();

        return new string(result);
    }

    private string GetKey(string keyGen, IEnumerable<KeyValuePair<string, string>> parameters)
    {
        var queryString = string.Join("&", parameters.Select(pair => $"{pair.Key}={pair.Value}"));
        var value = $"{queryString}{keyGen}";

        var hash = MD5Hash(value);

        return $"{GenerateString(6)}{hash}{GenerateString(6)}";
    }

    private string GetKeyGen(
        string agentId,
        string agentKey)
    {
        var date = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-4D)).ToString("yyMMd");
        var value = $"{date}{agentId}{agentKey}";

        return MD5Hash(value);
    }

    private string MD5Hash(string original)
    {
        var data = Encoding.UTF8.GetBytes(original);
        var hash = MD5.HashData(data);

        var sb = new StringBuilder();

        foreach (var v in hash)
            sb.Append(v.ToString("x2"));

        return sb.ToString();
    }
}
