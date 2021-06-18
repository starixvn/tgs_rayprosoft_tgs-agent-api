using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations.Queries;
using TGS.Services.Operations.Validators;

namespace TGS.Apis.AgentApi.Handlers.Agent
{
    public sealed class GetAgentHandler : IRequestHandler<GetAgentRequest, GetAgentResponse>
    {
        private readonly IAgentQueries _agentQueries;
        private readonly HttpContext _httpContext;
        private readonly IStatusValidator _statusValidator;

        public GetAgentHandler(
            IHttpContextAccessor httpContextAccessor,
            IAgentQueries agentQueries,
            IStatusValidator statusValidator)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _statusValidator = statusValidator;
            _agentQueries = agentQueries;
        }

        async Task<GetAgentResponse> IRequestHandler<GetAgentRequest, GetAgentResponse>.Handle(
            GetAgentRequest request, CancellationToken cancellationToken)
        {
            var brandId = _httpContext.Items.GetBrandId();
            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

            var agentTokenData = _httpContext.Items.GetAgentTokenData();
            var agentId = agentTokenData.AgentId;

            var agent = await _agentQueries.GetAgentAsync(x => x.Id == agentId);
            if (agent == null)
            {
                throw new PlayerNotFoundException(agentId.ToString(), Resources.Agent.NotFound);
            }

            return new()
            {
                Data = new()
                {
                    Username = agent.Username,
                    RealName = agent.RealName,
                    Email = agent.Email,
                    Phone = agent.Phone,
                    QQ = agent.QQ,
                    Wechat = agent.Wechat,
                    InvitationCode = agent.InvitationCode,
                    BrandUrl = agent.Brand.Domains.FirstOrDefault()?.Url,
                    IsSetPayPassword = agent.WithdrawPassword.Length > 0
                }
            };
        }
    }
}