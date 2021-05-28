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
                // sports 
                1 => ActivatorUtilities.CreateInstance<SportBetService>(_serviceProvider),

                // sbo rng
                3 => ActivatorUtilities.CreateInstance<RngBetService>(_serviceProvider),

                // virtual sports
                5 => ActivatorUtilities.CreateInstance<VirtualSportBetService>(_serviceProvider),

                _ => throw new NotImplementedException()
            };
    }
}