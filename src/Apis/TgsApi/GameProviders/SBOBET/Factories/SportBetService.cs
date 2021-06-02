using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
using TGS.Core.Infrastructures.Redis;
using TGS.Services.GameOperations;
using TGS.Services.Operations;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Factories
{
    internal sealed class SportBetService : BaseBetService
    {
        public SportBetService(
            IHttpContextAccessor httpContextAccessor,
            IGamePlayActions actions,
            ISBOBETActionModelMapper map,
            ISBOBETServices services,
            ISettingOperations settingOperations,
            IRedisCacheManager redisCacheManager)
            : base(
                httpContextAccessor.HttpContext,
                actions,
                map,
                services,
                settingOperations,
                redisCacheManager)
        {
        }
    }
}