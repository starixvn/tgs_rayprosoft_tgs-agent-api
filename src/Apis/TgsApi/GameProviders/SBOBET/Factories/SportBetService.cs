using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
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
            ISettingOperations settingOperations)
            : base(httpContextAccessor, actions, map,
                services, settingOperations)
        {
        }
    }
}