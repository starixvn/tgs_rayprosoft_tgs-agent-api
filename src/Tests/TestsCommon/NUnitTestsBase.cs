using NUnit.Framework;

namespace TGS.Tests.TestsCommon
{
    [TestFixture]
    public abstract class NUnitTestsBase
    {
        [SetUp]
        public virtual void RunBeforeEachTest()
        {
            InitializeBeforeEachTest();
        }

        protected abstract void InitializeBeforeEachTest();
    }
}