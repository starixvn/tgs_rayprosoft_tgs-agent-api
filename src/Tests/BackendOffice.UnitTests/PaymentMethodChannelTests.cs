using Moq;
using NUnit.Framework;
using TGS.Services.BackOffice.Application.Queries.BrandPayment;
using TGS.Services.Payment;

namespace TGS.Tests.BackendOffice.UnitTests
{
    [TestFixture]
    public class PaymentMethodChannelTests
    {
        [Test]
        public void Should_Get_All_PaymentMethodChannels()
        {
            var moqPaymentRepository = new Mock<IPaymentRepository>();

            var sut = new AllPaymentMethodChannelQueryService(moqPaymentRepository.Object);

            sut.QueryAsync(new AllPaymentMethodChannelQuery());

            moqPaymentRepository.Verify(
                fake => fake.GetPaymentMethodChannelsAsync(),
                Times.Exactly(1));
        }
    }
}
