using NUnit.Framework;

namespace TKS.Infrastructures.Extensions.UnitTests
{
    [TestFixture, Category(Named.Unit), Parallelizable]
    public class IntegerExtensionsTests
    {
        [Test]
        public void Can_Get_ToDollars()
        {
            12345.ToDollars().Should().Be(123.45m);
            2147483647.ToDollars().Should().Be(21474836.47m);
        }
    }
}