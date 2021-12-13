using NUnit.Framework;
using TGS.Apis.AgentApi.Models.Messages;
using TGS.Apis.AgentApi.Validators;

namespace Agent.UnitTests
{
    public class AgentPasswordTests
    {
        [Test]
        public void ChangePasswordRequestValidator_LengthCheck()
        {
            var sut = new ChangePasswordRequestValidator();

            var data = new ChangePasswordRequest { NewPassword = "12345" };

            var actual = sut.Validate(data);

            Assert.IsFalse(actual.IsValid);
        }
    }
}
