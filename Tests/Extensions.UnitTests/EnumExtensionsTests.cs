using NUnit.Framework;

namespace TKS.Infrastructures.Extensions.UnitTests
{
    [TestFixture, Category(Named.Unit), Parallelizable]
    public class EnumExtensionsTests
    {
        public enum EWeekDay
        {
            Sunday = 0,
            Monday = 1
        }

        [TestCase(EWeekDay.Sunday, "Sunday")]
        public void Can_Execute_Description(EWeekDay queryEWeekDay, string result)
        {
            queryEWeekDay.Description().Should().Be(result);
        }
    }
}