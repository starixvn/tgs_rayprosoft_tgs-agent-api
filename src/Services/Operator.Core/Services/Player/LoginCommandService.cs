using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Tokens;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Services.Operations.Validators;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Player
{
    internal class LoginCommandService : IGetService<LoginCommand, LoginInfo>
    {
        private readonly IStatusValidator _statusValidator;
        private readonly IPlayerOperations _playerOperations;
        private readonly IPlayerQueries _playerQueries;
        private readonly IPasswordHandler _passwordHandler;
        private readonly ITokenProvider _tokenProvider;
        private readonly IHashProvider _hashProvider;
        private readonly IJwtTokenHandler _jwtTokenHandler;
        private readonly IBrandOperations _brandOperations;
        private readonly IPlayerRepository _playerRepository;

        public LoginCommandService(
            IStatusValidator statusValidator,
            IPlayerOperations playerOperations,
            IPlayerQueries playerQueries,
            IPasswordHandler passwordHandler,
            ITokenProvider tokenProvider,
            IHashProvider hashProvider,
            IJwtTokenHandler jwtTokenHandler,
            IBrandOperations brandOperations,
            IPlayerRepository playerRepository)
        {
            _statusValidator = statusValidator ?? throw new ArgumentNullException(nameof(statusValidator));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _playerQueries = playerQueries ?? throw new ArgumentNullException(nameof(playerQueries));
            _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _hashProvider = hashProvider ?? throw new ArgumentNullException(nameof(hashProvider));
            _jwtTokenHandler = jwtTokenHandler ?? throw new ArgumentNullException(nameof(jwtTokenHandler));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public async ValueTask<LoginInfo> GetAsync(LoginCommand query, CancellationToken cancellationToken = default)
        {
            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(query.BrandId, ValidStatus.Default);

            var playerCode = await _playerOperations.GetPlayerCodeByUsernameAsync(query.BrandId, query.UserName);
            if (playerCode.IsNullOrEmpty())
            {
                throw new PlayerNotFoundException("");
            }

            var (salt, password) = await _playerQueries.GetSaltAndPasswordByPlayerCodeAsync(playerCode);
            if (!_passwordHandler.Verify(password, salt, query.Password))
            {
                throw new InvalidCredentialsException("");
            }

            _statusValidator.ThrowIfPlayerDisabled(PlayerStatus.Enabled);

            var createdOn = DateTimeOffset.UtcNow;
            var ipAddress = query.IpAddress;

            var record = await _playerOperations.PlayerLoginAsync(new()
            {
                BrandId = query.BrandId,
                Username = query.UserName,
                PlayerCode = playerCode,
                PlatformType = query.PlatformType,
                IpAddress = ipAddress,
                CreatedOn = createdOn,
                ExpiredOn = createdOn.AddSeconds(Consts.TokenExpireSeconds.PlayerLogin)
            });

            var token = await GeneratePlayerJwtTokenAsync(
                query.BrandId,
                new PlayerTokenData()
                {
                    PlayerCode = playerCode,
                    LoginRecordId = record.Id,
                    PlatformType = record.PlatformType,
                    ExpiredOn = record.ExpiredOn
                },
                query.Site);

            await _playerRepository.PlayerLoginIpAsync(playerCode, query.IpAddress);

            return new LoginInfo()
            {
                PlayerToken = token,
                LoginTime = createdOn.ToUnixTimeMilliseconds()
            };
        }

        private async Task<string> GeneratePlayerJwtTokenAsync(Guid brandId, PlayerTokenData playerTokenData, string site)
        {
            var data = await _tokenProvider.EncryptAsync(playerTokenData);

            var jti = Guid.NewGuid().ToString();
            var identity = new ClaimsIdentity(
                new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, data),
                    new Claim(JwtRegisteredClaimNames.Jti, jti)
                });

            var payload = _hashProvider.Hash($"{data}:{jti}");
            var secretKey = await _brandOperations.GetBrandSecretKeyAsync(brandId);
            var signature = Encoding.UTF8.GetBytes($"{payload}:{secretKey}");

            return _jwtTokenHandler.GenerateToken(
                identity,
                signature,
                site,
                playerTokenData.ExpiredOn.DateTime);
        }
    }
}