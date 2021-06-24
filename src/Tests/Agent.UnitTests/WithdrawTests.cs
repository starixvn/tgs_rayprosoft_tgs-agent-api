using System;
using System.Text.Json;
using System.Threading.Tasks;
using ExpectedObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using Tgs.Payment;
using TGS.Core.Infrastructures.Json;
using TGS.Services.Agent;
using TGS.Services.Agent.Services.Withdraw;
using TGS.Services.WalletService;

namespace Agent.UnitTests
{
    [TestFixture]
    public class WithdrawTests
    {
        [Test]
        public async Task Agent_Balance_Not_Enough_Create_Withdraw_Form_Will_Fail()
        {
            var fakeAgentRepository = Substitute.For<IAgentRepository>();
            var fakeWalletRepository = Substitute.For<IAgentWalletRepository>();
            var fakePaymentRepository = Substitute.For<IPaymentRepository>();
            var jsonSerializationProvider = new JsonSerializationProvider();
            var optionsAccessor = new OptionsWrapper<JsonOptions>(
                new JsonOptions());

            var sut = new CreateWithdrawFormCommandService(
                fakeAgentRepository,
                fakeWalletRepository,
                fakePaymentRepository,
                jsonSerializationProvider,
                optionsAccessor);

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
                    Guid.NewGuid(),
                    "test@gmail.com",
                    null,
                    null));

            fakeWalletRepository
                .GetBalanceAsync(Arg.Any<Guid>(), Arg.Is(command.AgentId))
                .Returns(2000M);

            var fakeBankInfo = new AgentBankInfo(
                command.BankInfoId,
                command.AgentId,
                command.BrandId,
                Guid.NewGuid(),
                "Bank",
                "{}");

            var expected = new WithdrawFormCreateInfo(
                command.AgentId.ToString(),
                "ThisIsAgentName",
                UserType.Agent,
                command.BrandId,
                command.Amount,
                7000,
                null,
                command.BankInfoId).ToExpectedObject();

            fakePaymentRepository
                .GetAgentBankInfoByIdAsync(Arg.Any<Guid>())
                .Returns(fakeBankInfo);

            fakePaymentRepository
                .When(fake => fake.CreateWithdrawFormAsync(
                    Arg.Any<Guid>(),
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

            _ = fakePaymentRepository
                .Received(1)
                .CreateWithdrawFormAsync(
                    Arg.Any<Guid>(),
                    Arg.Any<WithdrawFormCreateInfo>());
        }

        [Test]
        public async Task Agent_Can_Create_Withdraw_Form()
        {
            var fakeAgentRepository = Substitute.For<IAgentRepository>();
            var fakeWalletRepository = Substitute.For<IAgentWalletRepository>();
            var fakePaymentRepository = Substitute.For<IPaymentRepository>();
            var jsonSerializationProvider = new JsonSerializationProvider();
            var optionsAccessor = new OptionsWrapper<JsonOptions>(
                new JsonOptions());

            var sut = new CreateWithdrawFormCommandService(
                fakeAgentRepository,
                fakeWalletRepository,
                fakePaymentRepository,
                jsonSerializationProvider,
                optionsAccessor);

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
                    Guid.NewGuid(),
                    "test@gmail.com",
                    null,
                    null));

            fakeWalletRepository
                .GetBalanceAsync(Arg.Any<Guid>(), Arg.Is(command.AgentId))
                .Returns(2000M);

            var fakeBankInfo = new AgentBankInfo(
                command.BankInfoId,
                command.AgentId,
                command.BrandId,
                Guid.NewGuid(),
                "Bank",
                "{}");

            var expected = new WithdrawFormCreateInfo(
                command.AgentId.ToString(),
                "ThisIsAgentName",
                UserType.Agent,
                command.BrandId,
                command.Amount,
                1000,
                null,
                command.BankInfoId,
                BankInfo: jsonSerializationProvider.SerializeToString(
                    new
                    {
                        fakeBankInfo.Id,
                        fakeBankInfo.PaymentChannelId,
                        fakeBankInfo.BankType,
                        Detail = jsonSerializationProvider.DeserializeFromString<JsonDocument>(fakeBankInfo.Detail, null)
                    },
                    optionsAccessor.Value.JsonSerializerOptions)).ToExpectedObject();

            fakePaymentRepository
                .GetAgentBankInfoByIdAsync(Arg.Any<Guid>())
                .Returns(fakeBankInfo);

            fakePaymentRepository
                .When(fake => fake.CreateWithdrawFormAsync(
                    Arg.Any<Guid>(),
                    Arg.Any<WithdrawFormCreateInfo>()))
                .Do(callInfo =>
                {
                    var info = callInfo.Arg<WithdrawFormCreateInfo>();

                    expected.ShouldMatch(info);
                });

            await sut.ExecuteAsync(command);

            _ = fakePaymentRepository
                .Received(1)
                .CreateWithdrawFormAsync(
                    Arg.Any<Guid>(),
                    Arg.Any<WithdrawFormCreateInfo>());
        }
    }
}
