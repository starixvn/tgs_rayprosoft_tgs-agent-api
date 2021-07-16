using System;
using System.Text.Json;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.Payment;
using Tgs.Payment.Clients;
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
            var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();
            var fakeAgentRepository = Substitute.For<IAgentRepository>();

            var sut = new CreateBankInfoCommandService(fakePaymentServiceClient, fakeAgentRepository);

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
                .Returns(new AgentInfo(
                    agentId,
                    "Tester",
                    null,
                    null,
                    Guid.NewGuid(),
                    "test@gmail.com",
                    null,
                    null,
                    "aaabbb",
                    null,
                    Array.Empty<byte>(),
                    Array.Empty<byte>()));

            await sut.ExecuteAsync(command);

            _ = fakePaymentServiceClient
                .Received(1)
                .CreateAgentBankInfoAsync(
                    Arg.Is(command.BrandId),
                    Arg.Is(command.AgentId),
                    Arg.Any<string>(),
                    Arg.Is<BankCreateInfo>(info => info.PaymentChannelId == paymentChannelId));
        }

        [Test]
        public async Task Player_Can_Delete_BankInfo()
        {
            var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();

            var sut = new DeleteBankInfoCommandService(fakePaymentServiceClient);

            var agentId = Guid.NewGuid();
            var bankInfoId = Guid.NewGuid();
            var command = new DeleteBankInfoCommand(agentId, bankInfoId);

            await sut.ExecuteAsync(command);

            _ = fakePaymentServiceClient
                .Received(1)
                .DeleteAgentBankInfoAsync(Arg.Is(agentId), Arg.Is(bankInfoId));
        }

        [Test]
        public void Player_Can_List_Self_BankInfos()
        {
            var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();

            var sut = new GetAgentBankInfosQueryService(fakePaymentServiceClient);

            var agentId = Guid.NewGuid();

            sut.QueryAsync(new GetAgentBankInfosQuery(agentId));

            _ = fakePaymentServiceClient
                .Received(1)
                .GetAgentBankInfosAsync(Arg.Is(agentId));
        }
    }
}
