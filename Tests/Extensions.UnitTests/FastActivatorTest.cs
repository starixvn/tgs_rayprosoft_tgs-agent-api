using System;
using NUnit.Framework;

namespace TKS.Infrastructures.Extensions.UnitTests
{
    [TestFixture, Category(Named.Unit), Parallelizable]
    public class FastActivatorTest
    {
        [Test]
        public void CreateInstanceWithGenericConstaint()
        {
            FastActivator<Random>.CreateInstance().Should().NotBeNull();

            FastActivator<Guid>.CreateInstance().Should().Be(Guid.Empty);
        }

        [Test]
        public void CreateInstanceWithType()
        {
            FastActivator.CreateInstance(typeof(Random)).Should().BeOfType<Random>();

            FastActivator.CreateInstance(typeof(Guid)).Should().Be(Guid.Empty);
        }
    }
}