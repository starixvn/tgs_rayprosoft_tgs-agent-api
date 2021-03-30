using System;
using System.Threading.Tasks;
using ExpectedObjects;
using Moq;
using NUnit.Framework;
using TGS.Services.Operator;
using TGS.Services.Operator.BankDeposit;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class BankDepositTests
    {
        [Test]
        public async Task Create_Bank_Deposit_Form_Should_Success()
        {
            var moqDepositRepository = new Mock<IWalletRepository>();

            var sut = new CreateBankDepositFormCommandService(moqDepositRepository.Object);

            var command = new CreateBankDepositFormCommand(
                "ThisIsPlayerCode",
                Guid.NewGuid(),
                Guid.NewGuid(),
                "ThisIsBankAccount",
                1000M);
            var expected = new BankDepositForm(
                command.PlayerCode,
                command.BrandId,
                command.BrandPaymentId,
                command.BankAccount,
                command.Amount).ToExpectedObject();

            await sut.ExecuteAsync(command);

            moqDepositRepository.Verify(
                fake => fake.CreateDepositFormAsync(It.Is<BankDepositForm>(form => expected.Matches(form))),
                Times.Exactly(1));
        }
    }
}
