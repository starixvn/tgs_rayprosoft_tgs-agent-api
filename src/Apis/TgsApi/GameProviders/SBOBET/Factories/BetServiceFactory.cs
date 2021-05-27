using System;
using Microsoft.Extensions.DependencyInjection;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Factories
{
    internal class BetServiceFactory : IBetServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public BetServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IBetService Get(int productType) =>
            productType switch
            {
                1 or 5 => ActivatorUtilities.CreateInstance<SportBetService>(_serviceProvider),
                _      => ActivatorUtilities.CreateInstance<SportBetService>(_serviceProvider)
            };
    }
}