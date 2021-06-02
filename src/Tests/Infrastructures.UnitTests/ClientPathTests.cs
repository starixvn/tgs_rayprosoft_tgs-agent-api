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
                    year: 2021,
                    month: 6,
                    day: 2,
                    hour: 0,
                    minute: 0,
                    second: 0,
                    offset: TimeSpan.Zero)
            };

            var expected = "Time=2021-06-02T00:00:00.00Z";

            var actual = ClientPath.AppendWithProperties(string.Empty, parameter);

            Assert.AreEqual(expected, actual);
        }
    }
}
