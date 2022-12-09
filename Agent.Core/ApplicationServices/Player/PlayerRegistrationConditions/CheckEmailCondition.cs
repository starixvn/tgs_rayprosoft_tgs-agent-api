using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.PlayerService.Client;
using TGS.Services.Agent.Exceptions;

namespace TGS.Services.Agent.ApplicationServices.Player.PlayerRegistrationConditions
{
    internal class CheckEmailCondition : IRegistrationCondition
    {
        private readonly IPlayerServiceClient _playerServiceClient;

        public CheckEmailCondition(IPlayerServiceClient playerServiceClient)
        {
            _playerServiceClient = playerServiceClient ?? throw new ArgumentNullException(nameof(playerServiceClient));
        }

        public async ValueTask ExecuteAsync(Guid brandId, JsonDocument input,
            CancellationToken cancellationToken)
        {
            var emailData = input.Deserialize<EmailData>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

            var findPlayer = await _playerServiceClient
                .GetPlayerInfoByEmailAsync(brandId, emailData.Email, cancellationToken)
                .ConfigureAwait(false);

            if (findPlayer is not null)
                throw new PlayerEmailDuplicateException();
        }
    }
}
