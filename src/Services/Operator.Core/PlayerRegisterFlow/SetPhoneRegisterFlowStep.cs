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
    private readonly IGetService<BrandQuery, BrandInfo> _getBrandInfoService;
    private readonly IGetService<StartSetPlayerPhoneCommand, string> _startSetPlayerPhoneService;
    private readonly IPlayerServiceClient _playerServiceClient;

    public string Type => "InitPhone";

    public SetPhoneRegisterFlowStep(
        IGetService<BrandQuery, BrandInfo> getBrandInfoService,
        IGetService<CheckBrandPhoneQuery, bool> checkBrandPhoneService,
        IGetService<StartSetPlayerPhoneCommand, string> startSetPlayerPhoneService,
        IPlayerServiceClient playerServiceClient)
    {
        _getBrandInfoService = getBrandInfoService ?? throw new ArgumentNullException(nameof(getBrandInfoService));
        _checkBrandPhoneService = checkBrandPhoneService ?? throw new ArgumentNullException(nameof(checkBrandPhoneService));
        _startSetPlayerPhoneService = startSetPlayerPhoneService ?? throw new ArgumentNullException(nameof(startSetPlayerPhoneService));
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
        var request = input.Deserialize<InitPhoneInputData>()!;

        if (string.IsNullOrWhiteSpace(request.Phone)
            || string.IsNullOrWhiteSpace(request.AreaCode))
            throw new PlayerPhoneRequiredException();

        var brand = await _getBrandInfoService
            .GetAsync(new BrandQuery(brandId))
            .ConfigureAwait(false);

        if (!brand.SmsRegisterFlowEnabled)
            throw new InvalidOperationException();

        if (string.IsNullOrWhiteSpace(locationInfo.PlayerCode))
            throw new PlayerNotFoundException("Player not found.");

        var playerInfo = await _playerServiceClient
            .GetPlayerInfoAsync(locationInfo.PlayerCode, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (playerInfo is null)
            throw new PlayerNotFoundException("Player not found.");

        var checkQuery = new CheckBrandPhoneQuery(
            brand.BrandId,
            playerInfo.PlayerCode,
            request.AreaCode.Trim(),
            request.Phone.Trim());

        var isBrandHasSamePhone = await _checkBrandPhoneService
            .GetAsync(checkQuery)
            .ConfigureAwait(false);

        if (isBrandHasSamePhone)
            throw new BrandPhoneDuplicateException();

        var startSetPlayerPhoneCommand = new StartSetPlayerPhoneCommand(
            playerInfo.PlayerCode,
            request.AreaCode.Trim(),
            request.Phone.Trim(),
            CultureInfo.CurrentUICulture,
            brand.SmsEnabled);

        var result = await _startSetPlayerPhoneService
            .GetAsync(startSetPlayerPhoneCommand, cancellationToken)
            .ConfigureAwait(false);

        return new RegisterFlowProcessResult(
            true,
            null,
            JsonSerializer.SerializeToDocument(result));
    }

    internal record InitPhoneInputData
    {
        [JsonPropertyName("areacode")]
        public string AreaCode { get; init; } = default!;

        [JsonPropertyName("phone")]
        public string Phone { get; init; } = default!;
    }
}
