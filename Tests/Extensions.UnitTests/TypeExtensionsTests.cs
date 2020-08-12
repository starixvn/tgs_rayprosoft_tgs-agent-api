using NUnit.Framework;

namespace TKS.Infrastructures.Extensions.UnitTests
{
    [TestFixture, Category(Named.Unit), Parallelizable]
    public class TypeExtensionsTests
    {
        public class Test
        {
            public int Id { get; set; }
            public int Name { get; set; }

            public Test(int id, int name)
            {
                Id = id;
                Name = name;
            }
        }

        [Test]
        public void Can_Execute_GetPropertyInfo()
        {
            var testString = new Test(1, 2);

            var queryString = testString.GetType();
            var properInfo = queryString.GetPropertyInfo();

            queryString.GetPropertyInfo().Should().Be(properInfo);
        }
    }
}