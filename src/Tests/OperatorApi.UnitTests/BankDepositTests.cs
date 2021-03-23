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
            var moqDepositRepository = new Mock<IDepositRepository>();

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

        [Test]
        public async Task Get_Bank_Deposit_Form_By_Id_Should_Success()
        {
            var moqDepositRepository = new Mock<IDepositRepository>();

            var sut = new GetBankDepositFromQueryService(moqDepositRepository.Object);

            var query = new GetBankDepositFormQuery(
                "ThisIsPlayerCode",
                Guid.NewGuid());

            moqDepositRepository
                .Setup(fake => fake.GetDepositFormByIdAsync(It.Is<Guid>(x => x == query.FormId)))
                .Returns(ValueTask.FromResult(new BankDepositForm(
                    query.PlayerCode,
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    "test",
                    1000M)));

            var actual = await sut.GetAsync(query);

            moqDepositRepository.Verify(
                fake => fake.GetDepositFormByIdAsync(It.Is<Guid>(x => x == query.FormId)),
                Times.Exactly(1));
        }

        [Test]
        public void Bank_Deposit_Form_If_Not_Player_Own_Will_Throw_Exception()
        {
            var moqDepositRepository = new Mock<IDepositRepository>();

            var sut = new GetBankDepositFromQueryService(moqDepositRepository.Object);

            var query = new GetBankDepositFormQuery(
                "ThisIsPlayerCode",
                Guid.NewGuid());

            moqDepositRepository
                .Setup(fake => fake.GetDepositFormByIdAsync(It.Is<Guid>(x => x == query.FormId)))
                .Returns(ValueTask.FromResult(new BankDepositForm(
                    "OtherPlayerCode",
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    "test",
                    1000M)));

            Assert.ThrowsAsync<UltraViresException>(async () => await sut.GetAsync(query));
        }
    }
}
