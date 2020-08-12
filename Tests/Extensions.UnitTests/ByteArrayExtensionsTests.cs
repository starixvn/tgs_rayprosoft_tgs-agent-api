using System;
using FluentAssertions;
using NUnit.Framework;

namespace TKS.Infrastructures.Extensions.UnitTests
{
    [TestFixture, Category(Named.Unit), Parallelizable]
    public class ByteArrayExtensionsTests
    {
        [TestCase(new byte[] { 2, 4, 6, 8, 10, 12, 14, 16 })]
        public void Can_Get_ToBase64String_From_ByteArray(byte[] queryBytes)
        {
            Convert.ToBase64String(queryBytes).Should().Be(queryBytes.ToBase64String());
        }

        [TestCase(new byte[] { 2, 4, 6, 8, 10, 12, 14, 16 }, "020406080a0c0e10")]
        public void Can_Get_ToHexString_From_ByteArray(byte[] queryBytes, string result)
        {
            queryBytes.ToHexString().Should().Be(result);
        }
    }
}