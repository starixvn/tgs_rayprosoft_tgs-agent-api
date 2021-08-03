using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.ServiceUtility;
using CoreModels = TGS.Core.DataServices.Tgs.Models;

namespace TGS.Services.BackOffice.Application.Queries.BrandLiveChat
{
    internal class GetBrandLiveChatQueryService : IGetService<BrandLiveChatQuery, BrandLiveChatInfo>
    {
        private readonly ITgsQueries<CoreModels.BrandLiveChat> _tgsQueries;

        public GetBrandLiveChatQueryService(ITgsQueries<CoreModels.BrandLiveChat> tgsQueries)
        {
            _tgsQueries = tgsQueries ?? throw new ArgumentNullException(nameof(tgsQueries));
        }

        public async ValueTask<BrandLiveChatInfo> GetAsync(BrandLiveChatQuery query, CancellationToken cancellationToken = default)
        {
            var result = await _tgsQueries.GetQueries()
                .ToAsyncEnumerable()
                .FirstOrDefaultAsync(x => x.BrandId == query.BrandId);
            return new BrandLiveChatInfo()
            {
                ChatName = result.ChatName,
                ChatSetting = JsonDocument.Parse(result.ChatSetting)
            };
        }
    }
}