using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Services.Caches;
using Tgs.Payment;
using TGS.Services.Agent.Services.Withdraw;
using TGS.Services.Operations;
using TGS.Services.WalletService;
using Microsoft.AspNetCore.Mvc;
using TGS.Services.Agent;
using ExpectedObjects;

namespace Agent.UnitTests
{
    [TestFixture]
    public class WithdrawTests
    {
        [Test]
        public async Task Player_Can_Create_Withdraw_Form()
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
                UserType.Player,
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
                .When(fake => fake.CreateWithdrawFormAsync(Arg.Any<Guid>(), Arg.Any<WithdrawFormCreateInfo>()))
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
