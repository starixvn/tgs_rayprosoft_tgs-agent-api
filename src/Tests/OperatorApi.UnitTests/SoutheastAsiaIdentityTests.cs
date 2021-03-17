using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TGS.Services.Operator;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class SoutheastAsiaIdentityTests
    {
        [Test]
        public void SoutheastAsia_Identity_Always_Correct()
        {
            var sut = new SoutheastAsiaIdentityVerifier();

            var id = "anywords";

            var actual = sut.Verify(id);

            Assert.IsTrue(actual);
        }
    }
}
