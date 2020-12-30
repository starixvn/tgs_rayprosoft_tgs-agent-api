using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.GameProvider;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Queries.GameProvider
{
    public class GetGameProvidersQueryHandler : Handler<GetGameProvidersRequest, PagedResponse<GameProviderModel>>
    {
        private readonly IGameProviderService _service;

        public GetGameProvidersQueryHandler(IGameProviderService service, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
        }

        public override Task<PagedResponse<GameProviderModel>> Handle(GetGameProvidersRequest request,
            CancellationToken cancellationToken)
        {
            return _service.GetGameProvidersAsync()
                .Where(ResellerId != null, r => r.Resellers.Select(x => x.ResellerId).Contains(ResellerId.Value))
                .Where(LicenseeId != null, l => l.Licensees.Select(x => x.LicenseeId).Contains(LicenseeId.Value))
                .Where(BrandId != null, b => b.Brands.Select(x => x.BrandId).Contains(BrandId.Value))
                .Where(request.Code, g => g.Code == request.Code)
                .Where(request.Name, g => g.Name == request.Name)
                .Select(x => new GameProviderModel
                {
                    Id = x.Id,
                    UniqueId = x.UniqueId,
                    Name = x.Name,
                    Code = x.Code,
                    Status = x.Status,
                    Description = x.Description,
                    UpdatedByUsername = x.UpdatedByUsername,
                    UpdatedOn = x.UpdatedOn
                }).CreatePagedResponseAsync(request);
        }
    }
}