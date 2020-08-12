using FluentAssertions;
using NUnit.Framework;

namespace TKS.Infrastructures.Extensions.UnitTests
{
    [TestFixture, Category(Named.Unit), Parallelizable]
    public class DecimalExtensionsTests
    {
        [TestCase(5.6, 5.6)]
        [TestCase(-2.2, 2.2)]
        public void Can_Get_Absolute(decimal queryDecimal, decimal result)
        {
            queryDecimal.Absolute().Should().Be(result);
        }

        [TestCase(16.2, -16.2)]
        [TestCase(-7.1, 7.1)]
        public void Can_Get_Negate(decimal queryDecimal, decimal result)
        {
            queryDecimal.Negate().Should().Be(result);
        }

        [TestCase(3.4, 3, "3.400")]
        [TestCase(1.3657, 2, "1.37")]
        [TestCase(-2.429, 2, "-2.43")]
        public void Can_Get_ToDigits(decimal queryDecimal, int digit, string result)
        {
            queryDecimal.ToDigits(digit).Should().Be(result);
        }

        [TestCase(5.66973, 3, 5.669)]
        [TestCase(1.689543, 2, 1.68)]
        [TestCase(-2.21235, 1, -2.3)]
        public void Can_Get_Floor_By_Digit(decimal queryDecimal, int digit, decimal result)
        {
            queryDecimal.FloorByDigit(digit).Should().Be(result);
        }

        [Test]
        public void Can_Get_ToCents()
        {
            100m.ToCents().Should().Be(10000);
        }
    }
}