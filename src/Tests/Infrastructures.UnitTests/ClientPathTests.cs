using System;
using NUnit.Framework;
using TGS.Core.Infrastructures.Utility;

namespace Infrastructures.UnitTests
{
    [TestFixture]
    public class ClientPathTests
    {
        [Test]
        public void Client_Should_Process_Nullable_DateTimeOffset()
        {
            var parameter = new
            {
                Time = (DateTimeOffset?)new DateTimeOffset(
                    2021,
                    6,
                    2,
                    0,
                    0,
                    0,
                    TimeSpan.Zero)
            };

            var expected = "Time=2021-06-02 00:00:00Z";

            var actual = ClientPath.AppendWithProperties(string.Empty, parameter);

            Assert.AreEqual(expected, actual);
        }
    }
}