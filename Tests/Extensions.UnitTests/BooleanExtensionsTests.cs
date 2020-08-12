using NUnit.Framework;

namespace TKS.Infrastructures.Extensions.UnitTests
{
    [TestFixture, Category(Named.Unit), Parallelizable]
    public class BooleanExtensionsTests
    {
        [TestCase(true, "true")]
        [TestCase(false, "false")]
        public void Can_Get_ToLowerString(bool queryBool, string result)
        {
            queryBool.ToLowerString().Should().Be(result);
        }
    }
}