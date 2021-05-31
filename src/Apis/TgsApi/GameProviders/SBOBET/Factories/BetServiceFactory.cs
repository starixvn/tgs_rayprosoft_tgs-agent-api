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

                // sbo rng game
                3 => ActivatorUtilities.CreateInstance<RngGameBetService>(_serviceProvider),

                // virtual sports
                5 => ActivatorUtilities.CreateInstance<VirtualSportBetService>(_serviceProvider),

                // casino game
                7 => ActivatorUtilities.CreateInstance<CasinoBetService>(_serviceProvider),

                9 => ActivatorUtilities.CreateInstance<RngGameBetService>(_serviceProvider),

                _ => throw new NotImplementedException()
            };
    }
}