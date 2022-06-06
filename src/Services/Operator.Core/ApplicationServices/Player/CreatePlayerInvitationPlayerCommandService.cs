using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;
using TGS.Services.Operations.Models.Services;

namespace TGS.Services.Operator.ApplicationServices.Player
{
    internal class CreatePlayerInvitationPlayerCommandService : IGetService<CreatePlayerWithPlayerInvitationCommand, CreatePlayerInfo>
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IBrandRepository _brandRepository;
        private readonly IPasswordHandler _passwordHandler;
        private readonly IPlayerOperations _playerOperations;
        private readonly IPlayerRepository _playerRepository;

        public CreatePlayerInvitationPlayerCommandService(
            IPlayerOperations playerOperations,
            IPasswordHandler passwordHandler,
            IBrandOperations brandOperations,
            IPlayerRepository playerRepository,
            IBrandRepository brandRepository)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
        }

        public async ValueTask<CreatePlayerInfo> GetAsync(
            CreatePlayerWithPlayerInvitationCommand command,
            CancellationToken cancellationToken = default)
        {
            var player = await _playerRepository
                .GetPlayerInfoByUserNameAsync(command.BrandId, command.UserName)
                .ConfigureAwait(false);

            if (player != null)
                return new CreatePlayerInfo(player.PlayerCode, false);

            var (salt, password) = _passwordHandler.GenerateCredential(command.Password);

            var brand = await _brandRepository
                .GetBrandInfoAsync(command.BrandId)
                .ConfigureAwait(false) ?? throw new BrandNotFoundException();

            var betLimitId = await _brandOperations
                .GetBrandDefaultBetLimitAsync(command.BrandId, command.CurrencyCode)
                .ConfigureAwait(false);

            var playerStatus = brand.SmsRegisterFlowEnabled
                ? PlayerStatus.InBlacklist
                : PlayerStatus.Enabled;

            var resultPlayerCode = await CreatePlayerByUpperPlayerAsync(
                command,
                salt!,
                password!,
                command.CurrencyCode,
                betLimitId,
                playerStatus).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(resultPlayerCode) && !brand.SmsRegisterFlowEnabled)
            {
                await _playerRepository
                    .PlayerRegisterIpAsync(
                        resultPlayerCode,
                        command.IpAddress,
                        command.HostName,
                        DateTimeOffset.UtcNow)
                    .ConfigureAwait(false);

                await _playerRepository
                    .PlayerLoginIpAsync(
                        resultPlayerCode,
                        command.IpAddress,
                        command.HostName)
                    .ConfigureAwait(false);
            }

            return new CreatePlayerInfo(resultPlayerCode, true);
        }

        private async Task<string> CreatePlayerByUpperPlayerAsync(
            CreatePlayerWithPlayerInvitationCommand command,
            byte[] salt,
            byte[] password,
            string currency,
            int betLimitId,
            PlayerStatus status)
        {
            var upperPlayer = await _playerRepository
                .GetPlayerInfoAsync(command.UpperPlayerCode)
                .ConfigureAwait(false);

            if (upperPlayer == null || upperPlayer.BrandId != command.BrandId || upperPlayer.Status != PlayerStatus.Enabled)
                throw new InvalidInvitationCodeException(
                    command.UpperPlayerCode,
                    "Can't find the player info");

            var result = await _playerOperations
                .CreatePlayerAsync(
                    new CreatePlayerData
                    {
                        BrandId = command.BrandId,
                        UserName = command.UserName,
                        Salt = salt,
                        Password = password,
                        CurrencyCode = currency,
                        BetLimitId = betLimitId,
                        UpperPlayerCode = upperPlayer.PlayerCode,
                        Type = command.IsTestPlayer ? PlayerType.TestPlayer : PlayerType.RealPlayer,
                        PlayerStatus = status
                    })
                .ConfigureAwait(false);

            return result;
        }
    }
}
