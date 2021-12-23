using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;
using TGS.Core.Models.Enums;
using TGS.Services.Operations;
using TGS.Services.Operations.Models.Services;

namespace TGS.Services.Operator.Services.Player
{
    internal class CreatePlayerCommandService : IGetService<CreatePlayerCommand, CreatePlayerInfo>
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IBrandRepository _brandRepository;
        private readonly IPasswordHandler _passwordHandler;
        private readonly IPlayerOperations _playerOperations;
        private readonly IPlayerRepository _playerRepository;

        public CreatePlayerCommandService(
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
            CreatePlayerCommand command,
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

            var resultPlayerCode = await CreatePlayerAsync(
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
            }

            return new CreatePlayerInfo(resultPlayerCode, true);
        }

        private Task<string> CreatePlayerAsync(
            CreatePlayerCommand command,
            byte[] salt,
            byte[] password,
            string currency,
            int betLimitId,
            PlayerStatus status)
            => _playerOperations.CreatePlayerAsync(
                new CreatePlayerData
                {
                    BrandId = command.BrandId,
                    UserName = command.UserName,
                    Salt = salt,
                    Password = password,
                    CurrencyCode = currency,
                    BetLimitId = betLimitId,
                    Type = command.IsTestPlayer ? PlayerType.TestPlayer : PlayerType.RealPlayer,
                    PlayerStatus = status
                });
    }
}
