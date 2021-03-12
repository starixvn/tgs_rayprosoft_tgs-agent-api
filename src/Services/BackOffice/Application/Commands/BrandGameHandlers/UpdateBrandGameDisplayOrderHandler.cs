using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.BrandGameHandlers
{
    public class UpdateBrandGameDisplayOrderHandler : Handler<ChangeOrderRequest, MessageResponse>
    {
        private readonly IBrandGameService _service;

        public UpdateBrandGameDisplayOrderHandler(IBrandGameService service, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
        }

        public override async Task<MessageResponse> Handle(ChangeOrderRequest request,
            CancellationToken cancellationToken)
        {
            var ids = request.Orders.Select(q => q.Id);
            var games = await _service.GetBrandGameQueries(q => q.Where(x => ids.Contains(x.Id)))
                .ToListAsync(cancellationToken);
            games.ForEach(p => p.DisplayOrder = request.Orders.FirstOrDefault(x => x.Id == p.Id).DisplayOrder);

            await _service.UpdateBrandGamesAsync(games);

            return CreateValidResponse<MessageResponse>();
        }
    }
}