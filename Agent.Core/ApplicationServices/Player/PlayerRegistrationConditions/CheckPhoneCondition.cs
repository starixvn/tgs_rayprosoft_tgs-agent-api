using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.PlayerService.Client;
using TGS.Services.Agent.Exceptions;

namespace TGS.Services.Agent.ApplicationServices.Player.PlayerRegistrationConditions
{
    internal class CheckPhoneCondition(IPlayerServiceClient playerServiceClient) : IRegistrationCondition
    {
        private readonly IPlayerServiceClient _playerServiceClient = playerServiceClient ?? throw new ArgumentNullException(nameof(playerServiceClient));

        public async ValueTask ExecuteAsync(Guid brandId, JsonDocument input,
            CancellationToken cancellationToken)
        {
            var phoneData = input.Deserialize<PhoneData>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

            var findPlayer = await _playerServiceClient
                .GetPlayerInfoByPhoneAsync(
                    brandId,
                    phoneData.AreaCode.Trim(),
                    phoneData.Phone.Trim(),
                    cancellationToken)
                .ConfigureAwait(false);

            if (findPlayer is not null)
                throw new PlayerPhoneDuplicateException();
        }
    }
}
