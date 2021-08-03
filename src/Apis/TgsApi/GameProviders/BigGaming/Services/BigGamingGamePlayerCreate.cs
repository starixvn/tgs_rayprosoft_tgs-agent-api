using System;
using System.Threading.Tasks;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Shared.Game.ApiClients.BigGaming;

namespace TGS.Apis.TgsApi.GameProviders.BigGaming.Services
{
    public interface IBigGamingGamePlayerCreate
    {
        ValueTask<bool> CreatePlayerAsync(CachedPlayer player);
    }
    sealed class BigGamingGamePlayerCreate : IBigGamingGamePlayerCreate
    {
        private readonly ISettingOperations _settingOperations;
        private readonly IBigGamingService _service;

        public BigGamingGamePlayerCreate(
            ISettingOperations settingOperations,
            IBigGamingService service
            )
        {
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<bool> CreatePlayerAsync(CachedPlayer player)
        {
            var response = await _service.CreatePlayerAsync(player).ConfigureAwait(false);

            if (response.Error != null)
            {
                if (response.Error.Code == "2610")
                {
                    var isCreateAgent = await _service.CreateAgentAsync(player).ConfigureAwait(false);
                    if (!isCreateAgent)
                    {
                        throw new PlayerNotFoundException(player.PlayerCode);
                    }
                    response = await _service.CreatePlayerAsync(player).ConfigureAwait(false);
                    if (response.Error != null)
                    {
                        throw new PlayerNotFoundException(player.PlayerCode);
                    }

                }
                else
                {
                    throw new PlayerNotFoundException(player.PlayerCode);
                }
            }
            return true;
        }
    }
}
