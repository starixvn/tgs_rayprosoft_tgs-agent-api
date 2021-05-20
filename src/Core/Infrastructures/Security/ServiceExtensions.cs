using Microsoft.Extensions.DependencyInjection;

namespace TGS.Core.Infrastructures.Security
{
    public static class ServiceExtensions
    {
        /// <summary>
        ///     Dependency Injection of Infrastructures.Security, needs to add the following dependencies beforehand:
        ///     1. AddJson() in Infrastructures.Json
        /// </summary>
        public static IServiceCollection AddSecurity(this IServiceCollection services)
        {
            services.AddSingleton<IHashProvider, HashProvider>();
            services.AddSingleton<IPasswordHandler, PasswordHandler>();

            services.AddSingleton<DESCryptoProvider>();
            services.AddSingleton<AESCryptoProvider>();
            services.AddSingleton<AESBase62CryptoProvider>();

            services.AddSingleton<ISymmetricCryptoProviderFactory, SymmetricCryptoProviderFactory>();
            services.AddSingleton<ITokenProvider, TokenProvider>();

            services.AddSingleton<IJwtTokenHandler, JwtTokenHandler>();

            return services;
        }
    }
}