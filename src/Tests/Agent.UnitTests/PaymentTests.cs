using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Tgs.Payment;
using TGS.Services.Agent.Services.BrandPayments;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class PaymentTests
    {
        [Test]
        public async Task Get_Payment_Channels_Should_Success()
        {
            var moqPaymentRepository = new Mock<IPaymentRepository>();

            var sut = new AllPaymentChannelsQueryService(moqPaymentRepository.Object);

            var query = new AllPaymentChannelsQuery();

            moqPaymentRepository
                .Setup(fake => fake.GetAllPaymentChannelsAsync())
                .Returns(new[] {
                    new PaymentChannelInfo(Guid.NewGuid(), "test1"),
                    new PaymentChannelInfo(Guid.NewGuid(), "test2")
                }.ToAsyncEnumerable());

            _ = await sut.GetAsync(query);

            moqPaymentRepository.Verify(
                fake => fake.GetAllPaymentChannelsAsync(),
                Times.Exactly(1));
        }

        [Test]
        public async Task Get_Payment_Methods_Should_Success()
        {
            var moqPaymentRepository = new Mock<IPaymentRepository>();

            var sut = new AllPaymentMethodsQueryService(moqPaymentRepository.Object);

            var query = new AllPaymentMethodsQuery();

            moqPaymentRepository
                .Setup(fake => fake.GetAllPaymentMethodsAsync())
                .Returns(new[] {
                    new PaymentMethodInfo(Guid.NewGuid(), "test1", "Bank"),
                    new PaymentMethodInfo(Guid.NewGuid(), "test2", "Crypto")
                }.ToAsyncEnumerable());

            _ = await sut.GetAsync(query);

            moqPaymentRepository.Verify(
                fake => fake.GetAllPaymentMethodsAsync(),
                Times.Exactly(1));
        }

        [Test]
        public async Task User_Can_Get_PaymentInfo()
        {
            var moqPaymentRepository = new Mock<IPaymentRepository>();

            var sut = new GetPaymentQueryService(moqPaymentRepository.Object);

            var paymentId = Guid.NewGuid();

            var query = new GetPaymentQuery(paymentId);

            _ = await sut.GetAsync(query);

            moqPaymentRepository.Verify(
                fake => fake.GetPaymentAsync(It.Is<Guid>(x => x == paymentId)),
                Times.Exactly(1));
        }
    }
}
