using System;
using System.Text.Json;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.Payment;
using Tgs.Payment.Clients;
using TGS.Services.Operator.Services.Withdraw;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class BankInfoTests
    {
        [Test]
        public async Task Player_Can_Create_BankInfo()
        {
            var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();

            var sut = new CreateBankInfoCommandService(fakePaymentServiceClient);

            var brandId = Guid.NewGuid();
            var playerCode = "ThisIsPlayerCode";
            var paymentChannelId = Guid.NewGuid();
            var bankType = "Transfer";
            var detail = new
            {
                bankAccount = "0987654321"
            };

            var command = new CreateBankInfoCommand(brandId, playerCode, paymentChannelId, bankType, JsonSerializer.Serialize(detail));

            await sut.ExecuteAsync(command);

            _ = fakePaymentServiceClient
                .Received(1)
                .CreatePlayerBankInfoAsync(
                    Arg.Is(brandId),
                    Arg.Is(playerCode),
                    Arg.Is<BankCreateInfo>(info => info.PaymentChannelId == paymentChannelId));
        }

        [Test]
        public async Task Player_Can_Delete_BankInfo()
        {
            var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();

            var sut = new DeleteBankInfoCommandService(fakePaymentServiceClient);

            var playerCode = "ThisIsPlayerCode";
            var bankInfoId = Guid.NewGuid();
            var command = new DeleteBankInfoCommand(playerCode, bankInfoId);

            await sut.ExecuteAsync(command);

            _ = fakePaymentServiceClient
                .Received(1)
                .DeletePlayerBankInfoAsync(Arg.Is(playerCode), Arg.Is(bankInfoId));
        }

        [Test]
        public void Player_Can_List_Self_BankInfos()
        {
            var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();

            var sut = new GetPlayerBankInfosQueryService(fakePaymentServiceClient);

            var playerCode = "ThisIsPlayerCode";

            sut.QueryAsync(new GetPlayerBankInfosQuery(playerCode));

            _ = fakePaymentServiceClient.Received(1)
                .GetPlayerBankInfosAsync(Arg.Is(playerCode));
        }
    }
}
