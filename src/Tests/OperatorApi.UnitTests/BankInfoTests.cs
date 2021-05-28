using System;
using System.Text.Json;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.Services.Payment;
using TGS.Services.Operator.Services.Withdraw;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class BankInfoTests
    {
        [Test]
        public async Task Player_Can_Create_BankInfo()
        {
            var fakePaymentRepository = Substitute.For<IPaymentRepository>();

            var sut = new CreateBankInfoCommandService(fakePaymentRepository);

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

            _ = fakePaymentRepository
                .Received(1)
                .CreateBankInfoAsync(Arg.Is<BankInfo>(info => info.PaymentChannelId == paymentChannelId), Arg.Is(playerCode));
        }

        [Test]
        public async Task Player_Can_Delete_BankInfo()
        {
            var fakePaymentRepository = Substitute.For<IPaymentRepository>();

            var sut = new DeleteBankInfoCommandService(fakePaymentRepository);

            var playerCode = "ThisIsPlayerCode";
            var bankInfoId = Guid.NewGuid();
            var command = new DeleteBankInfoCommand(playerCode, bankInfoId);

            await sut.ExecuteAsync(command);

            _ = fakePaymentRepository
                .Received(1)
                .DeleteBankInfoAsync(Arg.Is(playerCode), Arg.Is(bankInfoId));
        }

        [Test]
        public void Player_Can_List_Self_BankInfos()
        {
            var fakePaymentRepository = Substitute.For<IPaymentRepository>();

            var sut = new GetPlayerBankInfosQueryService(fakePaymentRepository);

            var playerCode = "ThisIsPlayerCode";

            sut.QueryAsync(new GetPlayerBankInfosQuery(playerCode));

            _ = fakePaymentRepository
                .Received(1)
                .GetBankInfosByPlayerCodeAsync(Arg.Is(playerCode));
        }
    }
}
