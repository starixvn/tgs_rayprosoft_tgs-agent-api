using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Tokens;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Services.Operations.Validators;

namespace TGS.Apis.OperatorApi.Handlers.Player
{
    public sealed class LoginHandler : IRequestHandler<LoginRequest, LoginResponse>
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IHashProvider _hashProvider;
        private readonly HttpContext _httpContext;
        private readonly IJwtTokenHandler _jwtTokenHandler;
        private readonly IPasswordHandler _passwordHandler;
        private readonly IPlayerOperations _playerOperations;
        private readonly IPlayerQueries _playerQueries;
        private readonly IRequestAnalyzer _requestAnalyzer;
        private readonly IStatusValidator _statusValidator;
        private readonly ITokenProvider _tokenProvider;

        public LoginHandler(
            IHttpContextAccessor httpContextAccessor,
            IPlayerQueries playerQueries,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IJwtTokenHandler jwtTokenHandler,
            ITokenProvider tokenProvider,
            IHashProvider hashProvider,
            IPasswordHandler passwordHandler,
            IStatusValidator statusValidator,
            IRequestAnalyzer requestAnalyzer)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _tokenProvider = tokenProvider;
            _hashProvider = hashProvider;
            _passwordHandler = passwordHandler;
            _statusValidator = statusValidator;
            _playerQueries = playerQueries;
            _playerOperations = playerOperations;
            _brandOperations = brandOperations;
            _requestAnalyzer = requestAnalyzer;
            _jwtTokenHandler = jwtTokenHandler;
        }

        async Task<LoginResponse> IRequestHandler<LoginRequest, LoginResponse>.Handle(LoginRequest request,
            CancellationToken cancellationToken)
        {
            var brandId = _httpContext.Items.GetBrandId();
            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

            var username = request.Username;
            var playerCode = await _playerOperations.GetPlayerCodeByUsernameAsync(brandId, username);
            if (playerCode.IsNullOrEmpty())
            {
                throw new PlayerNotFoundException(username, Resources.Player.NotFound);
            }

            var (salt, password) = await _playerQueries.GetSaltAndPasswordByPlayerCodeAsync(playerCode);
            if (!_passwordHandler.Verify(password, salt, request.Password))
            {
                throw new InvalidCredentialsException(Resources.Player.PasswordMismatched);
            }

            _statusValidator.ThrowIfPlayerDisabled(PlayerStatus.Enabled);

            var createdOn = DateTimeOffset.UtcNow;
            var ipAddress = _requestAnalyzer.GetIp(_httpContext);

            var record = await _playerOperations.PlayerLoginAsync(new()
            {
                BrandId = brandId,
                Username = username,
                PlayerCode = playerCode,
                PlatformType = request.PlatformType,
                IpAddress = ipAddress,
                CreatedOn = createdOn,
                ExpiredOn = createdOn.AddSeconds(Core.Models.Consts.TokenExpireSeconds.PlayerLogin)
            });

            var token = await GeneratePlayerJwtTokenAsync(brandId, new()
            {
                PlayerCode = playerCode,
                LoginRecordId = record.Id,
                PlatformType = record.PlatformType,
                ExpiredOn = record.ExpiredOn
            });

            return new()
            {
                PlayerToken = token,
                LoginTime = createdOn.ToUnixTimeMilliseconds()
            };
        }

        private async Task<string> GeneratePlayerJwtTokenAsync(Guid brandId, PlayerTokenData playerTokenData)
        {
            var data = await _tokenProvider.EncryptAsync(playerTokenData);

            var jti = Guid.NewGuid().ToString();
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, data),
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            });

            var payload = _hashProvider.Hash($"{data}:{jti}");
            var secretKey = await _brandOperations.GetBrandSecretKeyAsync(brandId);
            var signature = Encoding.UTF8.GetBytes($"{payload}:{secretKey}");

            return _jwtTokenHandler.GenerateToken(identity, signature, Consts.Site,
                playerTokenData.ExpiredOn.DateTime);
        }
    }
}