using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Tgs.BrandCertificate.Clients;
using Tgs.ServiceUtility;
using TGS.Services.Operator.ApplicationServices.Player;

namespace TGS.Services.Operator;

internal class CreatePlayerRegisterFlowStep : IRegisterFlowStep
{
    private readonly IBrandServiceClient _brandServiceClient;
    private readonly IGetService<CreatePlayerWithAgentInvitationCodeCommand, CreatePlayerInfo> _createAgentInvitationPlayerService;
    private readonly IGetService<CreatePlayerWithPlayerInvitationCommand, CreatePlayerInfo> _createPlayerInvitationPlayerService;
    private readonly IGetService<CreatePlayerCommand, CreatePlayerInfo> _createPlayerService;
    public string Type => "CreatePlayer";

    public CreatePlayerRegisterFlowStep(
        IGetService<CreatePlayerCommand, CreatePlayerInfo> createPlayerService,
        IGetService<CreatePlayerWithAgentInvitationCodeCommand, CreatePlayerInfo> createAgentInvitationPlayerService,
        IGetService<CreatePlayerWithPlayerInvitationCommand, CreatePlayerInfo> createPlayerInvitationPlayerService,
        IBrandServiceClient brandServiceClient)
    {
        _createPlayerService = createPlayerService ?? throw new ArgumentNullException(nameof(createPlayerService));
        _createAgentInvitationPlayerService = createAgentInvitationPlayerService ?? throw new ArgumentNullException(nameof(createAgentInvitationPlayerService));
        _createPlayerInvitationPlayerService = createPlayerInvitationPlayerService ?? throw new ArgumentNullException(nameof(createPlayerInvitationPlayerService));
        _brandServiceClient = brandServiceClient ?? throw new ArgumentNullException(nameof(brandServiceClient));
    }

    public ValueTask<bool> ConfirmCompletedAsync(CancellationToken cancellationToken = default)
        => ValueTask.FromResult(false);

    public async ValueTask<RegisterFlowProcessResult> ProcessInputAsync(
        Guid brandId,
        JsonDocument input,
        PlayerLocationInfo locationInfo,
        CancellationToken cancellationToken = default)
    {
        var request = input.Deserialize<CreatePlayerInputData>()!;
        var brand = await _brandServiceClient
            .GetBrandInfoAsync(brandId, cancellationToken)
            .ConfigureAwait(false);

        if (!string.IsNullOrWhiteSpace(request.UpperPlayerCode))
        {
            var command = new CreatePlayerWithPlayerInvitationCommand(
                brandId,
                request.UserName,
                request.Password,
                brand!.Currency,
                request.UpperPlayerCode,
                request.IsTestPlayer,
                locationInfo.Ip,
                locationInfo.HostName);

            var info = await _createPlayerInvitationPlayerService
                .GetAsync(command)
                .ConfigureAwait(false);

            return new RegisterFlowProcessResult(
                info.IsNew,
                null,
                JsonSerializer.SerializeToDocument(
                    new CreatePlayerResponse
                    {
                        PlayerCode = info.PlayerCode,
                        IsNew = info.IsNew
                    }));
        }
        else if (!string.IsNullOrWhiteSpace(locationInfo.AgentInvitationCode))
        {
            var command = new CreatePlayerWithAgentInvitationCodeCommand(
                brandId,
                request.UserName,
                request.Password,
                brand!.Currency,
                locationInfo.AgentInvitationCode,
                request.IsTestPlayer,
                locationInfo.Ip,
                locationInfo.HostName);

            var info = await _createAgentInvitationPlayerService
                .GetAsync(command)
                .ConfigureAwait(false);

            return new RegisterFlowProcessResult(
                info.IsNew,
                null,
                JsonSerializer.SerializeToDocument(
                    new CreatePlayerResponse
                    {
                        PlayerCode = info.PlayerCode,
                        IsNew = info.IsNew
                    }));
        }
        else
        {
            var query = new CreatePlayerCommand(
                brandId,
                request.UserName,
                request.Password,
                brand!.Currency,
                request.IsTestPlayer,
                locationInfo.Ip,
                locationInfo.HostName);

            var info = await _createPlayerService.GetAsync(query).ConfigureAwait(false);

            return new RegisterFlowProcessResult(
                info.IsNew,
                null,
                JsonSerializer.SerializeToDocument(
                    new CreatePlayerResponse
                    {
                        PlayerCode = info.PlayerCode,
                        IsNew = info.IsNew
                    }));
        }
    }

    private record CreatePlayerInputData
    {
        [JsonPropertyName("invitationcode")]
        public string? AgentInvitationCode { get; init; }

        [JsonPropertyName("confirmpwd")]
        public string? ConfirmPassword { get; init; }

        [JsonPropertyName("testplayer")]
        public bool IsTestPlayer { get; init; } = false;

        [JsonPropertyName("pwd")]
        public string Password { get; init; } = default!;

        [JsonPropertyName("upper")]
        public string? UpperPlayerCode { get; init; }

        [Required]
        [JsonPropertyName("username")]
        public string UserName { get; init; } = default!;
    }

    private record CreatePlayerResponse
    {
        [JsonPropertyName("uid")]
        public string PlayerCode { get; init; } = default!;

        [JsonPropertyName("isnew")]
        public bool IsNew { get; init; }
    }
}
