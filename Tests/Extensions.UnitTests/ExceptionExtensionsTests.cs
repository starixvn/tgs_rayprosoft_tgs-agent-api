using System;
using NUnit.Framework;

namespace TKS.Infrastructures.Extensions.UnitTests
{
    [TestFixture, Category(Named.Unit), Parallelizable]
    public class ExceptionExtensionsTests
    {
        [Test]
        public void Can_Get_Null_When_Exception_Is_Null()
        {
            try
            {
                var rt = Convert.ToInt16("ABCD");
            }
            catch (Exception)
            {
                ((Exception)null).GetMessageChain().Should().Be(null);
            }
        }

        [Test]
        public void Can_Get_Message_Chain_When_InnerException_Is_NULL()
        {
            try
            {
                var rt = Convert.ToInt16("ABCD");
            }
            catch (Exception ex)
            {
                ex.GetMessageChain().Should().Be(ex.Message);
            }
        }

        [Test]
        public void Can_Get_Inner_Exception()
        {
            try
            {
                try
                {
                    var rt = Convert.ToInt16("ABCD");
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Current exception", ex);
                }
            }
            catch (Exception ex)
            {
                ex.GetMessageChain().Should().Contain("INNER 1");
            }
        }
    }
}