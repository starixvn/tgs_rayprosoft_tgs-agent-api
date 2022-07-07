using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Tgs.PlayerService.Client;

namespace TGS.Services.Operator.PlayerRegisterFlow;

internal class SetPhoneDontCareDuplicateRegisterFlowStep : IRegisterFlowStep
{
    private readonly IPlayerServiceClient _playerServiceClient;

    public string Type => "SetPhoneDontCareDuplicate";

    public SetPhoneDontCareDuplicateRegisterFlowStep(IPlayerServiceClient playerServiceClient)
    {
        _playerServiceClient = playerServiceClient ?? throw new ArgumentNullException(nameof(playerServiceClient));
    }

    public async ValueTask<bool> ConfirmCompletedAsync(string? playerCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(playerCode))
            return false;

        var player = await _playerServiceClient
            .GetPlayerInfoAsync(playerCode, cancellationToken)
            .ConfigureAwait(false);

        return player is not null && player.PhoneAuthentication;
    }

    public async ValueTask<RegisterFlowProcessResult> ProcessInputAsync(
        Guid brandId,
        JsonDocument input,
        PlayerLocationInfo locationInfo,
        CancellationToken cancellationToken = default)
    {
        var request = input.Deserialize<PhoneData>()!;

        if (string.IsNullOrWhiteSpace(locationInfo.PlayerCode))
            return new RegisterFlowProcessResult(false, null, null);

        await _playerServiceClient.SetPlayerPropertiesAsync(
            locationInfo.PlayerCode,
            new Dictionary<PlayerPropertyName, string?>
            {
                [PlayerPropertyName.AreaCode] = request.AreaCode,
                [PlayerPropertyName.Phone] = request.Phone,
                [PlayerPropertyName.PhoneAuthentication] = true.ToString()
            }.ToImmutableDictionary(),
            cancellationToken).ConfigureAwait(false);

        var player = await _playerServiceClient
            .GetPlayerInfoAsync(locationInfo.PlayerCode!, cancellationToken)
            .ConfigureAwait(false);

        return new RegisterFlowProcessResult(
            player is not null && player.PhoneAuthentication,
            null,
            null);
    }

    internal record PhoneData
    {
        [JsonPropertyName("areacode")]
        public string AreaCode { get; init; } = default!;

        [JsonPropertyName("phone")]
        public string Phone { get; init; } = default!;
    }
}
