using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tgs.ServiceUtility;
using TGS.Core.DataServices.Tgs;

namespace TGS.Services.Promotion.Services
{
    internal class ChangeOrderCommandService : ICommandService<ChangeOrderCommand>
    {
        private readonly ITgsRepository _tgsRepository;

        public ChangeOrderCommandService(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask ExecuteAsync(ChangeOrderCommand command, CancellationToken cancellationToken = default)
        {
            var ids = command.Orders.Select(q => q.Id);

            var promotions = await _tgsRepository.Promotions
                .AsQueryable()
                .Where(q => ids.Contains(q.Id))
                .ToListAsync()
                .ConfigureAwait(false);

            promotions.ForEach(p =>
            {
                var item = command.Orders.FirstOrDefault(x => x.Id == p.Id);

                if (item != null)
                {
                    p.DisplayOrder = item.DisplayOrder;
                }
            });

            _tgsRepository.Promotions.UpdateRange(promotions);
            _ = await _tgsRepository.SaveChangesAsync();
        }
    }
}
