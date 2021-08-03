using System;
using System.Threading.Tasks;
using ExpectedObjects;
using NSubstitute;
using NUnit.Framework;
using Tgs.Payment.Clients;
using TGS.Services.Agent;
using TGS.Services.Agent.Services.Withdraw;
using TGS.Services.WalletService;

namespace Agent.UnitTests
{
    [TestFixture]
    public class WithdrawTests
    {
        [Test]
        public void Agent_Balance_Not_Enough_Create_Withdraw_Form_Will_Fail()
        {
            var fakeAgentRepository = Substitute.For<IAgentRepository>();
            var fakeWalletRepository = Substitute.For<IAgentWalletRepository>();
            var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();

            var sut = new CreateWithdrawFormCommandService(
                fakeAgentRepository,
                fakeWalletRepository,
                fakePaymentServiceClient);

            var command = new CreateWithdrawFormCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                1000M);

            fakeAgentRepository
                .GetAgentByIdAsync(Arg.Is(command.AgentId))
                .Returns(new AgentInfo(
                    command.AgentId,
                    "ThisIsAgentName",
                    null,
                    null,
                    Guid.NewGuid(),
                    "test@gmail.com",
                    null,
                    null,
                    "aaabbb",
                    null,
                    1,
                    Array.Empty<byte>(),
                    Array.Empty<byte>(),
                    0M));

            fakeWalletRepository
                .GetBalanceAsync(Arg.Any<Guid>(), Arg.Is(command.AgentId))
                .Returns(2000M);

            var expected = new WithdrawFormCreateInfo
            {
                Amount = command.Amount,
                BankInfoId = command.BankInfoId,
                BrandId = command.BrandId,
                UserId = command.AgentId.ToString(),
                UserName = "ThisIsAgentName",
                UserType = UserType.Agent
            }.ToExpectedObject();

            fakePaymentServiceClient
                .When(fake => fake.CreateWithdrawFormAsync(
                    Arg.Any<WithdrawFormCreateInfo>()))
                .Do(callInfo =>
                {
                    var info = callInfo.Arg<WithdrawFormCreateInfo>();

                    expected.ShouldMatch(info);
                });

            fakeWalletRepository.ExchangeAsync(
                    Arg.Any<Guid>(),
                    Arg.Any<Guid>(),
                    Arg.Any<Guid>(),
                    Arg.Any<TransactionTypes>(),
                    Arg.Any<decimal>(),
                    Arg.Any<string>())
                .Returns(ValueTask.FromResult(
                    (Guid.NewGuid(), -100M)));

            Assert.ThrowsAsync<InsufficientFundsException>(
                async () => await sut.ExecuteAsync(command));

            _ = fakePaymentServiceClient
                .Received(1)
                .CreateWithdrawFormAsync(
                    Arg.Any<WithdrawFormCreateInfo>());
        }

        [Test]
        public async Task Agent_Can_Create_Withdraw_Form()
        {
            var fakeAgentRepository = Substitute.For<IAgentRepository>();
            var fakeWalletRepository = Substitute.For<IAgentWalletRepository>();
            var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();

            var sut = new CreateWithdrawFormCommandService(
                fakeAgentRepository,
                fakeWalletRepository,
                fakePaymentServiceClient);

            var command = new CreateWithdrawFormCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                1000M);

            fakeAgentRepository
                .GetAgentByIdAsync(Arg.Is(command.AgentId))
                .Returns(new AgentInfo(
                    command.AgentId,
                    "ThisIsAgentName",
                    null,
                    null,
                    Guid.NewGuid(),
                    "test@gmail.com",
                    null,
                    null,
                    "aaabbb",
                    null,
                    1,
                    Array.Empty<byte>(),
                    Array.Empty<byte>(),
                    0M));

            fakeWalletRepository
                .GetBalanceAsync(Arg.Any<Guid>(), Arg.Is(command.AgentId))
                .Returns(2000M);

            var expected = new WithdrawFormCreateInfo
            {
                Amount = command.Amount,
                BankInfoId = command.BankInfoId,
                BrandId = command.BrandId,
                UserId = command.AgentId.ToString(),
                UserName = "ThisIsAgentName",
                UserType = UserType.Agent
            }.ToExpectedObject();

            fakePaymentServiceClient
                .When(fake => fake.CreateWithdrawFormAsync(
                    Arg.Any<WithdrawFormCreateInfo>()))
                .Do(callInfo =>
                {
                    var info = callInfo.Arg<WithdrawFormCreateInfo>();

                    expected.ShouldMatch(info);
                });

            await sut.ExecuteAsync(command);

            _ = fakePaymentServiceClient
                .Received(1)
                .CreateWithdrawFormAsync(
                    Arg.Any<WithdrawFormCreateInfo>());
        }
    }
}