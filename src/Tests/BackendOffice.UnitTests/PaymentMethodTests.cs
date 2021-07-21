using Moq;
using NUnit.Framework;
using Tgs.Payment.Clients;
using TGS.Services.BackOffice.Application.Queries.PaymentMethod;

namespace TGS.Tests.BackendOffice.UnitTests
{
    [TestFixture]
    public class PaymentMethodTests
    {
        [Test]
        public void User_Can_Get_All_PaymentMethods()
        {
            var moqPaymentServiceClient = new Mock<IPaymentServiceClient>();

            var sut = new PaymentMethodQueryService(moqPaymentServiceClient.Object);

            var query = new PaymentMethodQuery();

            sut.QueryAsync(query);

            moqPaymentServiceClient.Verify(
                fake => fake.GetAllPaymentMethodsAsync(),
                Times.Exactly(1));
        }
    }
}
