using Moq;
using NUnit.Framework;
using Tgs.Payment;
using TGS.Services.BackOffice.Application.Queries.PaymentChannel;

namespace TGS.Tests.BackendOffice.UnitTests
{
    [TestFixture]
    public class PaymentChannelTests
    {
        [Test]
        public void User_Can_Get_All_PaymentChannels()
        {
            var moqPaymentRepository = new Mock<IPaymentRepository>();

            var sut = new PaymentChannelQueryService(moqPaymentRepository.Object);

            sut.QueryAsync(new PaymentChannelQuery());

            moqPaymentRepository.Verify(
                fake => fake.GetAllPaymentChannelsAsync(),
                Times.Exactly(1));
        }
    }
}
