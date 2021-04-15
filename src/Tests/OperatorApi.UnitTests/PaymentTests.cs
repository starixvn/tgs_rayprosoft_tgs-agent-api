using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TGS.Services.Operator;
using TGS.Services.Operator.Deposit;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class PaymentTests
    {
        [Test]
        public async Task Get_Payment_Methods_Should_Success()
        {
            var moqWalletRepository = new Mock<IWalletRepository>();

            var sut = new AllPaymentMethodsQueryService(moqWalletRepository.Object);

            var query = new AllPaymentMethodsQuery();

            moqWalletRepository
                .Setup(fake => fake.GetAllPaymentMethodsAsync())
                .Returns(new[] {
                    new PaymentMethodInfo(Guid.NewGuid(), "test1"),
                    new PaymentMethodInfo(Guid.NewGuid(), "test2")
                }.ToAsyncEnumerable());

            _ = await sut.GetAsync(query);

            moqWalletRepository.Verify(
                fake => fake.GetAllPaymentMethodsAsync(),
                Times.Exactly(1));
        }

        [Test]
        public async Task Get_Payment_Channels_Should_Success()
        {
            var moqWalletRepository = new Mock<IWalletRepository>();

            var sut = new AllPaymentChannelsQueryService(moqWalletRepository.Object);

            var query = new AllPaymentChannelsQuery();

            moqWalletRepository
                .Setup(fake => fake.GetAllPaymentChannelsAsync())
                .Returns(new[] {
                    new PaymentChannelInfo(Guid.NewGuid(), "test1"),
                    new PaymentChannelInfo(Guid.NewGuid(), "test2")
                }.ToAsyncEnumerable());

            _ = await sut.GetAsync(query);

            moqWalletRepository.Verify(
                fake => fake.GetAllPaymentChannelsAsync(),
                Times.Exactly(1));
        }

        [Test]
        public async Task User_Can_Get_PaymentInfo()
        {
            var moqWalletRepository = new Mock<IWalletRepository>();

            var sut = new GetPaymentQueryService(moqWalletRepository.Object);

            var paymentId = Guid.NewGuid();

            var query = new GetPaymentQuery(paymentId);

            _ = await sut.GetAsync(query);

            moqWalletRepository.Verify(
                fake => fake.GetPaymentAsync(It.Is<Guid>(x => x == paymentId)),
                Times.Exactly(1));
        }

        [Test]
        public void User_Can_List_Available_BrandPayments()
        {
            var moqWalletRepository = new Mock<IWalletRepository>();

            var sut = new BrandPaymentsQueryService(moqWalletRepository.Object);

            var brandId = Guid.NewGuid();
            var playerCode = "ThisIsPlayerCode";

            var query = new BrandPaymentsQuery(brandId, playerCode);

            _ = sut.QueryAsync(query);

            moqWalletRepository.Verify(
                fake => fake.GetBrandPaymentsAsync(It.Is<Guid>(x => x == brandId), It.Is<string>(x => x == playerCode)),
                Times.Exactly(1));
        }
    }
}
