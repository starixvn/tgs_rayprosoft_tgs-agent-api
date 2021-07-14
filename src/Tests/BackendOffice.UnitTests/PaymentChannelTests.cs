using Moq;
using NUnit.Framework;
using Tgs.Payment.Clients;
using TGS.Services.BackOffice.Application.Queries.PaymentChannel;

namespace TGS.Tests.BackendOffice.UnitTests
{
    [TestFixture]
    public class PaymentChannelTests
    {
        [Test]
        public void User_Can_Get_All_PaymentChannels()
        {
            var moqPaymentServiceClient = new Mock<IPaymentServiceClient>();

            var sut = new PaymentChannelQueryService(moqPaymentServiceClient.Object);

            sut.QueryAsync(new PaymentChannelQuery());

            moqPaymentServiceClient.Verify(
                fake => fake.GetAllPaymentChannelsAsync(),
                Times.Exactly(1));
        }
    }
}
