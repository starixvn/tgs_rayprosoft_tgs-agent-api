using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Tgs.Payment.Clients;
using TGS.Services.Agent.Services.BrandPayments;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class PaymentTests
    {
        [Test]
        public async Task Get_Payment_Channels_Should_Success()
        {
            var moqPaymentServiceClient = new Mock<IPaymentServiceClient>();

            var sut = new AllPaymentChannelsQueryService(moqPaymentServiceClient.Object);

            var query = new AllPaymentChannelsQuery();

            moqPaymentServiceClient
                .Setup(fake => fake.GetAllPaymentChannelsAsync())
                .Returns(new[] {
                    new PaymentChannelInfo
                    {
                        Id = Guid.NewGuid(),
                        Name = "test1"
                    },
                    new PaymentChannelInfo
                    {
                        Id = Guid.NewGuid(),
                        Name = "test2"
                    }
                }.ToAsyncEnumerable());

            _ = await sut.GetAsync(query);

            moqPaymentServiceClient.Verify(
                fake => fake.GetAllPaymentChannelsAsync(),
                Times.Exactly(1));
        }

        [Test]
        public async Task Get_Payment_Methods_Should_Success()
        {
            var moqPaymentServiceClient = new Mock<IPaymentServiceClient>();

            var sut = new AllPaymentMethodsQueryService(moqPaymentServiceClient.Object);

            var query = new AllPaymentMethodsQuery();

            moqPaymentServiceClient
                .Setup(fake => fake.GetAllPaymentMethodsAsync())
                .Returns(new[] {
                    new PaymentMethodInfo
                    {
                        Id = Guid.NewGuid(),
                        Name = "Bank"
                    } ,
                    new PaymentMethodInfo
                    {
                        Id = Guid.NewGuid(),
                        Name= "Crypto"
                    }
                }.ToAsyncEnumerable());

            _ = await sut.GetAsync(query);

            moqPaymentServiceClient.Verify(
                fake => fake.GetAllPaymentMethodsAsync(),
                Times.Exactly(1));
        }

        [Test]
        public async Task User_Can_Get_PaymentInfo()
        {
            var moqPaymentServiceClient = new Mock<IPaymentServiceClient>();

            var sut = new GetPaymentQueryService(moqPaymentServiceClient.Object);

            var paymentId = Guid.NewGuid();

            var query = new GetPaymentQuery(paymentId);

            _ = await sut.GetAsync(query);

            moqPaymentServiceClient.Verify(
                fake => fake.GetPaymentByIdAsync(It.Is<Guid>(x => x == paymentId)),
                Times.Exactly(1));
        }
    }
}
