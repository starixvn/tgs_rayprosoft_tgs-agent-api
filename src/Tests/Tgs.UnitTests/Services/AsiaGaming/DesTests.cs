using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;

namespace TGS.UnitTests.Services.AsiaGaming
{
    [TestFixture]
    public class DesTests
    {
        private readonly ISymmetricCryptoProvider _cryptoProvider;

        public DesTests()
        {
            var services = new ServiceCollection();
            services.AddSecurity();
            var sp = services.BuildServiceProvider();

            var cryptoProviderFactory = sp.GetService<ISymmetricCryptoProviderFactory>();
            _cryptoProvider = cryptoProviderFactory.GetProvider(SymmetricAlgorithmKind.DES);
        }
        
        [Test]
        public async Task Can_EncryptAsync()
        {
            var original = "cagent=81288128/\\\\\\\\/method=tc";
            var key = "12341234";
            var expectedResult = "IGcOAYEQN88F1NFLtBOK29IcQSW2a8b/G8UgSaeEyaA=";

            var result = await _cryptoProvider.EncryptStringAsync(original, key, key, 
                CipherMode.ECB, PaddingMode.PKCS7, Encoding.UTF8);
            result.Should().BeEquivalentTo(expectedResult);
        }
        
        [Test]
        public async Task Can_DecryptAsync()
        {
            var encrypted = "IGcOAYEQN88F1NFLtBOK29IcQSW2a8b/G8UgSaeEyaA=";
            var key = "12341234";
            var expectedResult = "cagent=81288128/\\\\\\\\/method=tc";
        
            var result = await _cryptoProvider.DecryptStringAsync(encrypted, key, key, 
                CipherMode.ECB, PaddingMode.PKCS7, Encoding.UTF8);
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}