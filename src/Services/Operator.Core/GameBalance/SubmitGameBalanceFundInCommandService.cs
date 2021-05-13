using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TGS.Services.Operations;
using TGS.Services.PluggableServices;
using TGS.Services.PluggableServices.Models;
using TGS.Services.WalletService;
using TGS.ServiceUtility;


namespace TGS.Services.Operator.GameBalance
{
    internal class SubmitGameBalanceFundInCommandService : ICommandService<SubmitGameBalanceFundInCommand>
    {        
        private readonly IPluggalbeGameProviderApiClient _pluggalbe;
        //private readonly IPluggableOperations _pluggableOperations;

        public SubmitGameBalanceFundInCommandService(IPluggalbeGameProviderApiClient walletRepository)
        {            
            _pluggalbe = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));            
        }

        public async ValueTask ExecuteAsync(SubmitGameBalanceFundInCommand command, CancellationToken cancellationToken = default)
        {

            
            //var language =
            //    await _brandLanguageCodeMapper.ExternalToInternalAsync(request.LanguageCode, cachedPlayer.BrandId);
            //var serviceUrl = await _pluggableOperations.GetEndpointUrlByGameProviderCodeAsync(command.GameProviderCode) ;
            //var r = await _pluggableApiClient.GetGameUrlAsync(serviceUrl, new()
            //{
            //    Token = playerTokenData,
            //    GameProviderCode = gameProviderCode,
            //    GameId = gameId,
            //    Language = language
            //});


            var apiUrl = new Uri("");
            await _pluggalbe.FundInAsync( apiUrl, new GameProviderWalletFundRequest() { GameProviderCode = command.GameProviderCode, PlayerCode = command.PlayerCode, Amount = command.Amount }) //(request.GameProviderCode).FundInAsync(request)
            .ConfigureAwait(false);
        }

    }
}