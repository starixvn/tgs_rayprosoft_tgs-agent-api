using System;
using System.Threading.Tasks;
using NUnit.Framework;
using TGS.Services.Operator.Withdraw;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class WithdrawTests
    {
        [Test]
        public async Task Player_Can_Create_Withdraw_Form()
        {
            var sut = new CreateWithdrawFormCommandService();

            var command = new CreateWithdrawFormCommand(
                "ThisIsPlayerCode",
                Guid.NewGuid(),
                Guid.NewGuid(),
                1000M);

            await sut.ExecuteAsync(command);

            Assert.Fail();
        }
    }
}
