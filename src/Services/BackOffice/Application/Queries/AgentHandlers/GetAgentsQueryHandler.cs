using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Agent;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Queries.AgentHandlers
{
    public class GetAgentsQueryHandler : Handler<GetAgentsRequest, PagedResponse<AgentModel>>
    {
        private readonly IAgentService _service;

        public GetAgentsQueryHandler(IAgentService service, IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
        }

        public override Task<PagedResponse<AgentModel>> Handle(GetAgentsRequest request,
            CancellationToken cancellationToken) =>
            _service.GetAgentQueries(q => q.Where(ResellerId, r => r.Brand.Licensee.ResellerId == ResellerId)
                    .Where(LicenseeId, l => l.Brand.LicenseeId == LicenseeId)
                    .Where(BrandId, l => l.Id == BrandId)
                    .Where(x => x.BrandId == request.BrandId)
                    .Where(request.Status, x => x.Status == request.Status)
                    .Where(request.LevelType, x => x.LevelType == request.LevelType)
                    .Where(request.Username, x => EF.Functions.ILike(x.Username, $"%{request.Username}%"))
                )
                .Select(x => new AgentModel
                {
                    Id = x.Id,
                    BrandId = x.BrandId,
                    Username = x.Username,
                    Status = x.Status,
                    AgentStatus = x.AgentStatus,
                    Type = x.Type,
                    LevelType = x.LevelType,
                    CommissionPercentage = x.CommissionPercentage,
                    DividendPercentage = x.DividendPercentage,
                    LiveDealerRebatePercentage = x.LiveDealerRebatePercentage,
                    SlotRebatePercentage = x.SlotRebatePercentage,
                    LotteryRebatePercentage = x.LotteryRebatePercentage,
                    CardGameRebatePercentage = x.CardGameRebatePercentage,
                    SportsRebatePercentage = x.SportsRebatePercentage,
                    VirtualSportsRebatePercentage = x.VirtualSportsRebatePercentage,
                    TeamNumber = 1 + x.Players.Count + x.SubAgents.Count +
                        x.SubAgents.SelectMany(s => s.Players).Count()
                }).CreatePagedResponseAsync(request);
    }
}