using System;
using System.Threading;
using System.Threading.Tasks;
using ExpectedObjects;
using NSubstitute;
using NUnit.Framework;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models;
using Tgs.Agent.Client.Models.Enums;
using Tgs.CashService.Client;
using Tgs.Payment.Clients;
using TGS.Services.Agent.ApplicationServices.Withdraw;
using TGS.Services.WalletService;
using InsufficientFundsException = TGS.Services.Agent.Exceptions.InsufficientFundsException;

namespace Agent.UnitTests;

[TestFixture]
public class WithdrawTests
{
    [Test]
    public void Agent_Balance_Not_Enough_Create_Withdraw_Form_Will_Fail()
    {
        var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();
        var fakeAgentClient = Substitute.For<IAgentClient>();
        var fakeCashServiceClient = Substitute.For<ICashServiceClient>();

        var sut = new CreateWithdrawFormCommandService(
            fakePaymentServiceClient,
            fakeCashServiceClient,
            fakeAgentClient);

        var command = new CreateWithdrawFormCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            1000M);

        fakeAgentClient
            .GetAgentInfoAsync(Arg.Any<GetAgentInfoRequest>())
            .Returns(new AgentInfoModel(
                command.AgentId,
                "ThisIsAgentName",
                "",
                "",
                "",
                "",
                "",
                "code",
                null,
                DateTimeOffset.Now,
                (AgentStatus)TGS.Core.Models.Enums.AgentStatus.Enabled,
                (AgentType)TGS.Core.Models.Enums.AgentType.Cash,
                (AgentLevelType)TGS.Core.Models.Enums.AgentLevelType.Agent,
                10,
                1,
                1,
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                true,
                true));

        fakeCashServiceClient
            .GetBalanceAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(-2000M);

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

        Assert.ThrowsAsync<InsufficientFundsException>(
            async () => await sut.ExecuteAsync(command));

        _ = fakePaymentServiceClient
            .Received(0)
            .CreateWithdrawFormAsync(
                Arg.Any<WithdrawFormCreateInfo>());
    }

    [Test]
    public async Task Agent_Can_Create_Withdraw_Form()
    {
        var fakeWalletRepository = Substitute.For<IAgentWalletRepository>();
        var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();
        var fakeAgentClient = Substitute.For<IAgentClient>();
        var fakeCashServiceClient = Substitute.For<ICashServiceClient>();

        var sut = new CreateWithdrawFormCommandService(
            fakePaymentServiceClient,
            fakeCashServiceClient,
            fakeAgentClient);

        var command = new CreateWithdrawFormCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            1000M);

        fakeAgentClient
            .GetAgentInfoAsync(Arg.Any<GetAgentInfoRequest>())
            .Returns(new AgentInfoModel(
                command.AgentId,
                "ThisIsAgentName",
                "",
                "",
                "",
                "",
                "",
                "code",
                null,
                DateTimeOffset.Now,
                (AgentStatus)TGS.Core.Models.Enums.AgentStatus.Enabled,
                (AgentType)TGS.Core.Models.Enums.AgentType.Cash,
                (AgentLevelType)TGS.Core.Models.Enums.AgentLevelType.Agent,
                10,
                1,
                1,
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                true,
                true));

        fakeCashServiceClient
            .GetBalanceAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(2000M);

        fakeCashServiceClient.LockWithdrawAsync(default, 0, null, null)
            .ReturnsForAnyArgs(new TransactionResponse { Balance = 2222 });

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