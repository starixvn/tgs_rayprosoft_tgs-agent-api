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
using Tgs.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Tokens;
using TGS.Services.Agent.Exceptions;
using TGS.Services.Operations;
using TGS.Services.Operations.Models.Services;
using TGS.Services.Operations.Queries;
using TGS.Services.Operations.Validators;
using AgentNotFoundException = TGS.Core.Models.Exceptions.AgentNotFoundException;

namespace TGS.Apis.AgentApi.Handlers.Agent;

public sealed class LoginHandler(
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
    : IRequestHandler<LoginRequest, LoginResponse>
{
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext;

    async Task<LoginResponse> IRequestHandler<LoginRequest, LoginResponse>.Handle(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var brandId = _httpContext.Items.GetBrandId();
        await statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

        var username = request.Username;
        var agent = await agentQueries.GetAgentAsync(x => x.BrandId == brandId && x.Username == username);

        if (agent is null) throw new AgentNotFoundException(username, Resources.Agent.NotFound);

        if (agent.Status == AgentStatus.InBlacklist) throw new UserBannedException();

        if (!passwordHandler.Verify(agent.Password, agent.Salt, request.Password))
            throw new InvalidCredentialsException(Resources.Agent.PasswordMismatched);

        statusValidator.ThrowIfPlayerDisabled(PlayerStatus.Enabled);

        var createdOn = DateTimeOffset.UtcNow;
        var ipAddress = requestAnalyzer.GetIp(_httpContext);

        var record = await agentOperations.AgentLoginAsync(new AgentLoginData
        {
            BrandId = brandId,
            Username = username,
            AgentId = agent.Id,
            IpAddress = ipAddress,
            CreatedOn = createdOn,
            ExpiredOn = createdOn.AddSeconds(Core.Models.Consts.AgentTokenExpireSeconds.AgentLogin)
        });

        var token = await GenerateAgentJwtTokenAsync(brandId,
            new AgentTokenData { AgentId = agent.Id, LoginRecordId = record.Id, ExpiredOn = record.ExpiredOn });

        return new LoginResponse { AgentToken = token, LoginTime = createdOn.ToUnixTimeMilliseconds() };
    }

    private async Task<string> GenerateAgentJwtTokenAsync(Guid brandId, AgentTokenData agentTokenData)
    {
        var data = await tokenProvider.EncryptAsync(agentTokenData);

        var jti = Guid.NewGuid().ToString();

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, data), new Claim(JwtRegisteredClaimNames.Jti, jti)
        });

        var payload = hashProvider.Hash($"{data}:{jti}");
        var secretKey = await brandOperations.GetBrandSecretKeyAsync(brandId);
        var signature = Encoding.UTF8.GetBytes($"{payload}:{secretKey}");

        return jwtTokenHandler.GenerateToken(identity,
            signature,
            Consts.Site,
            agentTokenData.ExpiredOn.DateTime);
    }
}