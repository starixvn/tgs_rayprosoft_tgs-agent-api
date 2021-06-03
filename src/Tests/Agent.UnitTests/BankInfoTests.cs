using System;
using System.Text.Json;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.Payment;
using TGS.Services.Agent;
using TGS.Services.Agent.Services.BankInfos;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class BankInfoTests
    {
        [Test]
        public async Task Player_Can_Create_BankInfo()
        {
            var fakePaymentRepository = Substitute.For<IPaymentRepository>();
            var fakeAgentRepository = Substitute.For<IAgentRepository>();

            var sut = new CreateBankInfoCommandService(fakePaymentRepository, fakeAgentRepository);

            var brandId = Guid.NewGuid();
            var agentId = Guid.NewGuid();
            var paymentChannelId = Guid.NewGuid();
            var bankType = "Transfer";
            var detail = new
            {
                bankAccount = "0987654321"
            };

            var command = new CreateBankInfoCommand(
                brandId,
                agentId,
                paymentChannelId,
                bankType,
                JsonSerializer.Serialize(detail));

            fakeAgentRepository
                .GetAgentByIdAsync(Arg.Is(agentId))
                .Returns(new AgentInfo(agentId, "Tester"));

            await sut.ExecuteAsync(command);

            _ = fakePaymentRepository
                .Received(1)
                .CreateAgentBankInfoAsync(
                    Arg.Is<AgentBankInfo>(info => info.PaymentChannelId == paymentChannelId),
                    Arg.Any<string>());
        }

        [Test]
        public async Task Player_Can_Delete_BankInfo()
        {
            var fakePaymentRepository = Substitute.For<IPaymentRepository>();

            var sut = new DeleteBankInfoCommandService(fakePaymentRepository);

            var agentId = Guid.NewGuid();
            var bankInfoId = Guid.NewGuid();
            var command = new DeleteBankInfoCommand(agentId, bankInfoId);

            await sut.ExecuteAsync(command);

            _ = fakePaymentRepository
                .Received(1)
                .DeleteAgentBankInfoAsync(Arg.Is(agentId), Arg.Is(bankInfoId));
        }

        [Test]
        public void Player_Can_List_Self_BankInfos()
        {
            var fakePaymentRepository = Substitute.For<IPaymentRepository>();

            var sut = new GetAgentBankInfosQueryService(fakePaymentRepository);

            var agentId = Guid.NewGuid();

            sut.QueryAsync(new GetAgentBankInfosQuery(agentId));

            _ = fakePaymentRepository
                .Received(1)
                .GetAgentBankInfosByAgentIdAsync(Arg.Is(agentId));
        }
    }
}
