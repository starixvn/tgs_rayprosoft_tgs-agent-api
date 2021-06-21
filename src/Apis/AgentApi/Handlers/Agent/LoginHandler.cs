using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Tokens;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Services.Operations.Validators;

namespace TGS.Apis.AgentApi.Handlers.Agent
{
    public sealed class LoginHandler : IRequestHandler<LoginRequest, LoginResponse>
    {
        private readonly IAgentOperations _agentOperations;
        private readonly IAgentQueries _agentQueries;
        private readonly IBrandOperations _brandOperations;
        private readonly IHashProvider _hashProvider;
        private readonly HttpContext _httpContext;
        private readonly IJwtTokenHandler _jwtTokenHandler;
        private readonly IPasswordHandler _passwordHandler;
        private readonly IRequestAnalyzer _requestAnalyzer;
        private readonly IStatusValidator _statusValidator;
        private readonly ITokenProvider _tokenProvider;

        public LoginHandler(
            IHttpContextAccessor httpContextAccessor,
            IAgentQueries agentQueries,
            IAgentOperations agentOperations,
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
            _agentQueries = agentQueries;
            _agentOperations = agentOperations;
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
            var agent = await _agentQueries.GetAgentAsync(x => x.BrandId == brandId && x.Username == username);
            if (agent is null)
            {
                throw new AgentNotFoundException(username, Resources.Agent.NotFound);
            }

            if (!_passwordHandler.Verify(agent.Password, agent.Salt, request.Password))
            {
                throw new InvalidCredentialsException(Resources.Agent.PasswordMismatched);
            }

            _statusValidator.ThrowIfPlayerDisabled(PlayerStatus.Enabled);

            var createdOn = DateTimeOffset.UtcNow;
            var ipAddress = _requestAnalyzer.GetIp(_httpContext);

            var record = await _agentOperations.AgentLoginAsync(new()
            {
                BrandId = brandId,
                Username = username,
                AgentId = agent.Id,
                IpAddress = ipAddress,
                CreatedOn = createdOn,
                ExpiredOn = createdOn.AddSeconds(Core.Models.Consts.AgentTokenExpireSeconds.AgentLogin)
            });

            var token = await GenerateAgentJwtTokenAsync(brandId,
                new() {AgentId = agent.Id, LoginRecordId = record.Id, ExpiredOn = record.ExpiredOn});

            return new() {AgentToken = token, LoginTime = createdOn.ToUnixTimeMilliseconds()};
        }

        private async Task<string> GenerateAgentJwtTokenAsync(Guid brandId, AgentTokenData agentTokenData)
        {
            var data = await _tokenProvider.EncryptAsync(agentTokenData);

            var jti = Guid.NewGuid().ToString();
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, data), new Claim(JwtRegisteredClaimNames.Jti, jti)
            });

            var payload = _hashProvider.Hash($"{data}:{jti}");
            var secretKey = await _brandOperations.GetBrandSecretKeyAsync(brandId);
            var signature = Encoding.UTF8.GetBytes($"{payload}:{secretKey}");

            return _jwtTokenHandler.GenerateToken(identity, signature, Consts.Site,
                agentTokenData.ExpiredOn.DateTime);
        }
    }
}