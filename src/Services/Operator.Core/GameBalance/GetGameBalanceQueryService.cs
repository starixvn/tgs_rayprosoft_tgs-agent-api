using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Operations;
using TGS.Services.Operator.GameBalance;
using TGS.Services.PluggableServices;
using TGS.Services.WalletService;
using TGS.ServiceUtility;
using TGS.Services.PluggableServices.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Services.Operator.GameBalance
{
    internal class GetGameBalanceQueryService : IGetService<GetGameBalanceQuery, decimal>
    {
        //private readonly IWalletRepository _walletRepository;
        //private readonly IPlayerOperations _playerOperations;

        private readonly IPluggalbeGameProviderApiClient _client;
        //private readonly IPluggableOperations _pluggableOperations;
        private readonly ISettingOperations _settingOperations;
        //public GetGameBalanceQueryService(IWalletRepository walletRepository, IPlayerOperations playerOperations)
        //{
        //    _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        //    _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
        //}


        public GetGameBalanceQueryService(IPluggalbeGameProviderApiClient client, ISettingOperations settingOperations)
        {
            _client = client;
            _settingOperations = settingOperations;
        }



        public async ValueTask<decimal> GetAsync(GetGameBalanceQuery query, CancellationToken cancellationToken = default)
        {

            //var url = await _pluggableOperations.GetEndpointUrlByGameProviderCodeAsync(query.gpcode);
            //var info = await _client.GetBalanceAsync(url, new GameProviderWalletGetBalanceRequest() {
            //    GameProviderCode = query.gpcode,
            //    PlayerCode = query.playercode }).ConfigureAwait(false);
            var config = await _settingOperations.GetValueFromJsonByKeyAsync<PluggableConfigurationBase>(
                SettingConsts.GameProviderConfigurations.Template.Args(query.playercode));
            var url =  config.ServiceEndpointUrl;

            var info = await _client.GetBalanceAsync(url, new GameProviderWalletGetBalanceRequest() { GameProviderCode = query.gpcode, PlayerCode = query.playercode });

            return info.Balance;   
        }
    }
}
