using FluentAssertions;
using NUnit.Framework;
using TGS.Core.Infrastructures.Extensions;

namespace Infrastructures.UnitTests
{
    [TestFixture]
    public class DecimalTests
    {
        [Test]
        public void DecimalTruncateTests()
        {
            10.6651M.Truncate(2).Should().Be(10.66m);
            10.6653M.Truncate(2).Should().Be(10.66m);
            10.6655M.Truncate(2).Should().Be(10.66m);
            10.6657M.Truncate(3).Should().Be(10.665m);
            10.665789M.Truncate(3).Should().Be(10.665m);
            10.66599999M.Truncate(3).Should().Be(10.665m);

            10.66599999M.Truncate(0).Should().Be(10m);
            10.66599999M.Truncate(1).Should().Be(10.6m);
            10.66599999M.Truncate(2).Should().Be(10.66m);
            10.66599999M.Truncate(3).Should().Be(10.665m);
            10.66599999M.Truncate(4).Should().Be(10.6659m);
        }
    }
}