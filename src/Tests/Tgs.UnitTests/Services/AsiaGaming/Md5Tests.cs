using System.Text;
using FluentAssertions;
using NUnit.Framework;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;

namespace TGS.UnitTests.Services.AsiaGaming
{
    [TestFixture]
    public class Md5Tests
    {
        private readonly IHashProvider _hashProvider;

        public Md5Tests()
        {
            _hashProvider = new HashProvider();
        }
        
        [Test]
        public void Can_Encrypt()
        {
            var plainText = "aaa";
            var expectedResult = "47bce5c74f589f4867dbd57e9ca9f808";
        
            var result = _hashProvider.Hash(plainText, HashAlgorithmKind.Md5, HashStringKind.Hex, Encoding.UTF8);
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}