using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Tgs.PlayerService.Client;
using Tgs.ServiceUtility;
using TGS.Apis.OperatorApi.Models;
using TGS.Services.Operator.ApplicationServices.Brand;
using TGS.Services.Operator.ApplicationServices.Player;

namespace TGS.Services.Operator.PlayerRegisterFlow;

internal class SetPhoneRegisterFlowStep : IRegisterFlowStep
{
    private readonly IGetService<CheckBrandPhoneQuery, bool> _checkBrandPhoneService;
    private readonly ICommandService<CommitInitPlayerPhoneCommand> _commitPlayerInitPhoneService;
    private readonly IGetService<BrandQuery, BrandInfo> _getBrandInfoService;
    private readonly IPlayerServiceClient _playerServiceClient;
    private readonly IGetService<StartSetPlayerPhoneCommand, string> _startSetPlayerPhoneService;
    public string Type => "InitPhone";

    public SetPhoneRegisterFlowStep(
        IGetService<BrandQuery, BrandInfo> getBrandInfoService,
        IGetService<CheckBrandPhoneQuery, bool> checkBrandPhoneService,
        IGetService<StartSetPlayerPhoneCommand, string> startSetPlayerPhoneService,
        ICommandService<CommitInitPlayerPhoneCommand> commitPlayerInitPhoneService,
        IPlayerServiceClient playerServiceClient)
    {
        _getBrandInfoService = getBrandInfoService ?? throw new ArgumentNullException(nameof(getBrandInfoService));
        _checkBrandPhoneService = checkBrandPhoneService ?? throw new ArgumentNullException(nameof(checkBrandPhoneService));
        _startSetPlayerPhoneService = startSetPlayerPhoneService ?? throw new ArgumentNullException(nameof(startSetPlayerPhoneService));
        _commitPlayerInitPhoneService = commitPlayerInitPhoneService ?? throw new ArgumentNullException(nameof(commitPlayerInitPhoneService));
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

    public ValueTask<RegisterFlowProcessResult> ProcessInputAsync(
        Guid brandId,
        JsonDocument input,
        PlayerLocationInfo locationInfo,
        CancellationToken cancellationToken = default)
    {
        var request = input.Deserialize<InitPhoneStepInputData>()!;

        if (request.InputType == "Valid")
            return ProcessValidPhoneAsync(
                brandId,
                request.Data.Deserialize<ConfirmPhoneData>()!,
                locationInfo,
                cancellationToken);
        else
            return ProcessSetPhoneAsync(
                brandId,
                request.Data.Deserialize<PhoneData>()!,
                locationInfo,
                cancellationToken);
    }

    private async ValueTask<RegisterFlowProcessResult> ProcessSetPhoneAsync(
        Guid brandId,
        PhoneData data,
        PlayerLocationInfo locationInfo,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(data.Phone)
            || string.IsNullOrWhiteSpace(data.AreaCode))
            throw new PlayerPhoneRequiredException();

        var brand = await _getBrandInfoService
            .GetAsync(new BrandQuery(brandId), cancellationToken)
            .ConfigureAwait(false);

        if (!brand.SmsRegisterFlowEnabled)
            throw new InvalidOperationException();

        if (string.IsNullOrWhiteSpace(locationInfo.PlayerCode))
            throw new PlayerNotFoundException("Player not found.");

        var playerInfo = await _playerServiceClient
            .GetPlayerInfoAsync(locationInfo.PlayerCode, cancellationToken)
            .ConfigureAwait(false);

        if (playerInfo is null)
            throw new PlayerNotFoundException("Player not found.");

        var checkQuery = new CheckBrandPhoneQuery(
            brand.BrandId,
            playerInfo.PlayerCode,
            data.AreaCode.Trim(),
            data.Phone.Trim());

        var isBrandHasSamePhone = await _checkBrandPhoneService
            .GetAsync(checkQuery, cancellationToken)
            .ConfigureAwait(false);

        if (isBrandHasSamePhone)
            throw new BrandPhoneDuplicateException();

        var startSetPlayerPhoneCommand = new StartSetPlayerPhoneCommand(
            playerInfo.PlayerCode,
            data.AreaCode.Trim(),
            data.Phone.Trim(),
            CultureInfo.CurrentUICulture,
            brand.SmsEnabled);

        var result = await _startSetPlayerPhoneService
            .GetAsync(startSetPlayerPhoneCommand, cancellationToken)
            .ConfigureAwait(false);

        return new RegisterFlowProcessResult(
            false,
            null,
            JsonSerializer.SerializeToDocument(result));
    }

    private async ValueTask<RegisterFlowProcessResult> ProcessValidPhoneAsync(
        Guid brandId,
        ConfirmPhoneData data,
        PlayerLocationInfo locationInfo,
        CancellationToken cancellationToken = default)
    {
        var command = new CommitInitPlayerPhoneCommand(
            data.SecurityKey,
            data.ValidateCode,
            locationInfo.Ip,
            locationInfo.HostName!,
            brandId);

        await _commitPlayerInitPhoneService
            .ExecuteAsync(command, cancellationToken)
            .ConfigureAwait(false);

        return new RegisterFlowProcessResult(
            true,
            null,
            null);
    }

    internal record InitPhoneStepInputData
    {
        [JsonPropertyName("input")]
        public string InputType { get; init; } = "Init";

        [JsonPropertyName("data")]
        public JsonDocument Data { get; init; } = default!;
    };

    internal record PhoneData
    {
        [JsonPropertyName("areacode")]
        public string AreaCode { get; init; } = default!;

        [JsonPropertyName("phone")]
        public string Phone { get; init; } = default!;
    }

    internal record ConfirmPhoneData
    {
        [JsonPropertyName("securitykey")]
        public string SecurityKey { get; init; } = default!;

        [JsonPropertyName("validatecode")]
        public string ValidateCode { get; init; } = default!;
    }
}
