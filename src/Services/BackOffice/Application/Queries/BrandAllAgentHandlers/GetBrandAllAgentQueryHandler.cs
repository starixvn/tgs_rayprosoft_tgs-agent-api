//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using TGS.Core.DataServices.Base.Extensions;
//using TGS.Core.Models.Messages;
//using TGS.Services.BackOffice.Application.Models.Agent;
//using TGS.Services.BackOffice.Application.Models.BrandAllAgent;
//using TGS.Services.BackOffice.Infrastructure.DataService;

//namespace TGS.Services.BackOffice.Application.Queries.BrandAllAgentHandlers
//{
//    public class GetBrandAllAgentQueryHandler : Handler<GetBrandAllAgentRequest, BrandAllAgentModel>
//    {
//        private readonly IAgentService _service;

//        public GetBrandAllAgentQueryHandler(IAgentService service, IHttpContextAccessor accessor) : base(accessor)
//        {
//            _service = service;
//        }

//        public override async Task<BrandAllAgentModel> Handle(GetBrandAllAgentRequest request,
//            CancellationToken cancellationToken)
//        {
//            var (agents, totalCount) = await _service.GetAgentsAsync(request);

//            return await agents.CreatePagedResponseAsync(totalCount);
//        }
//    }
//}

