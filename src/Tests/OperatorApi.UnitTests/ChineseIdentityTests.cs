using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TGS.Core.Infrastructures.Utility;
using TGS.Services.Operator;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class ChineseIdentityTests
    {
        [Test]
        public void Id_Length_15_Verify_Should_Correct()
        {
            var sut = new ChineseIdentity15Verifier();

            var id = "320101000101105";

            var actual = sut.Verify(id);

            Assert.IsTrue(actual);
        }

        [Test]
        public void Id_Length_18_Verify_Should_Correct()
        {
            var sut = new ChineseIdentity18Verifier();

            var id = "22010119000101191X";

            var actual = sut.Verify(id);

            Assert.IsTrue(actual);
        }
    }
}
