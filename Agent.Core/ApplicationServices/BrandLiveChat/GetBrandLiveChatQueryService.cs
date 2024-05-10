using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.BrandLiveChat;

internal class GetBrandLiveChatQueryService(ITgsRepository tgsRepository)
    : IGetService<BrandLiveChatQuery, BrandLiveChatInfo?>
{
    private readonly ITgsRepository _tgsRepository =
        tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));

    public async ValueTask<BrandLiveChatInfo?> GetAsync(
        BrandLiveChatQuery query,
        CancellationToken cancellationToken = default)
    {
        var brandLiveChat = await _tgsRepository.BrandLiveChats
            .FirstOrDefaultAsync(x => x.BrandId == query.BrandId);
        if (brandLiveChat == null) return null;

        return new BrandLiveChatInfo { Name = brandLiveChat.ChatName, Setting = brandLiveChat.ChatSetting };
    }
}