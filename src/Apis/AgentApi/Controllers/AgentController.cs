using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Services.Agent;
using TGS.Services.Agent.Services.Agent;
using TGS.Services.Operations.Validators;
using TGS.ServiceUtility;
using AgentDividendConditionModel = TGS.Services.Agent.Services.Agent.AgentDividendConditionModel;

namespace TGS.Apis.AgentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class AgentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AgentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        [CheckAgentEnabled]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<CreateAgentResponse> CreateAgentAsync(
            [FromServices] IGetService<GetCreateAgentQuery, CreateAgentInfo> getCreateAgentQueryService,
            [FromBody] CreateAgentRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();
            var agentTokenData = HttpContext.Items.GetAgentTokenData();

            var query = new GetCreateAgentQuery
            {
                BrandId = brandId,
                CardGameRebatePercentage = request.CardGameRebatePercentage,
                LiveDealerRebatePercentage = request.LiveDealerRebatePercentage,
                LotteryRebatePercentage = request.LotteryRebatePercentage,
                ParentAgentId = agentTokenData.AgentId,
                Password = request.Password,
                RealName = request.RealName,
                Email = request.Email,
                Phone = request.Phone,
                Remarks = request.Remarks,
                SlotRebatePercentage = request.SlotRebatePercentage,
                SportsRebatePercentage = request.SportsRebatePercentage,
                Username = request.UserName,
                VirtualSportsRebatePercentage = request.VirtualSportsRebatePercentage,
                AgentDividendConditions = request.AgentDividendConditions.Select(x => new AgentDividendConditionModel
                {
                    Rank = x.Rank,
                    NetProfit = x.NetProfit,
                    ActiveNumber = x.ActiveNumber,
                    DividendPercentage = x.DividendPercentage
                }).ToArray()
            };

            var info = await getCreateAgentQueryService
                .GetAsync(query)
                .ConfigureAwait(false);

            return new CreateAgentResponse
            {
                IsNew = info.IsNew,
                AgentId = info.AgentId,
                Message = info.Message
            };
        }

        /// <summary>
        /// Get an agent information
        /// </summary>
        [HttpGet("info")]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<GetAgentResponse> GetAgentInfoAsync(
            [FromServices] IStatusValidator statusValidator,
            [FromServices] IGetService<GetAgentInfoQuery, AgentInfo> getAgentService)
        {
            var brandId = HttpContext.Items.GetBrandId();
            await statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

            var agentTokenData = HttpContext.Items.GetAgentTokenData();
            var agentId = agentTokenData.AgentId;

            var agentInfo = await getAgentService.GetAsync(new GetAgentInfoQuery(agentId));

            if (agentInfo == null)
            {
                throw new AgentNotFoundException(agentId.ToString(), Resources.Agent.NotFound);
            }

            return new GetAgentResponse
            {
                Data = new AgentData
                {
                    Username = agentInfo.Username,
                    RealName = agentInfo.RealName,
                    Email = agentInfo.Email,
                    Phone = agentInfo.Phone,
                    QQ = agentInfo.QQ,
                    Wechat = agentInfo.Wechat,
                    InvitationCode = agentInfo.InvitationCode,
                    BrandUrl = agentInfo.BrandUrl,
                    IsSetPayPassword = agentInfo.PayPassword.Length > 0,
                    AgentLevel = agentInfo.AgentLevel,
                    AccumulatedProfitSharing = agentInfo.AccumulatedProfitSharing,
                    DividendPercentage = agentInfo.DividendPercentage,
                    ParentUsername = agentInfo.ParentUserName,
                    RegisteredOn = agentInfo.RegisteredOn,
                    Balance = agentInfo.Balance,
                    Status = agentInfo.Status.ToString(),
                    Type = agentInfo.Type.ToString(),
                    LevelType = agentInfo.AgentLevelType.ToString(),
                    DividendConditions = agentInfo.AgentDividendConditions.Select(x => new DividendConditionModel
                    {
                        Rank = x.Rank,
                        NetProfit = x.NetProfit,
                        ActiveNumber = x.ActiveNumber,
                        DividendPercentage = x.DividendPercentage
                    }).OrderBy(x => x.Rank).ToArray()
                }
            };
        }

        /// <summary>
        /// Agent login
        /// </summary>
        [HttpPost]
        [Route("login")]
        [Consumes(MediaTypeNames.Application.Json)]
        public Task<LoginResponse> LoginAsync(LoginRequest request) =>
            _mediator.Send(request);
    }
}
