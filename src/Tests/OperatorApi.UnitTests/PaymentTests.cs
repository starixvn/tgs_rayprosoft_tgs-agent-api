using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NSubstitute;
using NUnit.Framework;
using TGS.Services.Operator;
using TGS.Services.Payment;
using TGS.Services.Operator.Services.Deposit;

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
            // arrange
            // act
            // assert
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

        [Test]
        public async Task User_Can_List_Available_BrandPayments()
        {
            var fakePaymentRepository = Substitute.For<IPaymentRepository>();
            var fakeWalletRepository = Substitute.For<IPlayerRepository>();

            var sut = new BrandPaymentsQueryService(fakePaymentRepository, fakeWalletRepository);

            var brandId = Guid.NewGuid();
            var playerCode = "ThisIsPlayerCode";

            var query = new BrandPaymentsQuery(brandId, playerCode);

            fakePaymentRepository
                .GetBrandPaymentAclsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<string>())
                .Returns(Array.Empty<BrandPaymentAcl>().ToAsyncEnumerable());

            _ = await sut.QueryAsync(query).ToArrayAsync();

            fakePaymentRepository
                .Received(1)
                .GetBrandPaymentAclsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<string>());
        }
    }
}
