using System;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.Infrastructures.Security.Enums;

namespace TGS.Core.Infrastructures.Security
{
    public interface ISymmetricCryptoProviderFactory
    {
        ISymmetricCryptoProvider GetProvider(SymmetricAlgorithmKind algorithm);
    }

    public sealed class SymmetricCryptoProviderFactory : ISymmetricCryptoProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SymmetricCryptoProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ISymmetricCryptoProvider GetProvider(SymmetricAlgorithmKind algorithm) =>
            algorithm switch
            {
                SymmetricAlgorithmKind.DES => _serviceProvider.GetRequiredService<DESCryptoProvider>(),
                _                          => _serviceProvider.GetRequiredService<AESCryptoProvider>()
            };
    }
}
