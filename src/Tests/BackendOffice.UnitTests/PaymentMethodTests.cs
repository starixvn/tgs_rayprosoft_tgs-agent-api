using Moq;
using NUnit.Framework;
using TGS.Services.BackOffice.Application.Queries.PaymentMethod;
using TGS.Services.Payment;

namespace TGS.Tests.BackendOffice.UnitTests
{
    [TestFixture]
    public class PaymentMethodTests
    {
        [Test]
        public void User_Can_Get_All_PaymentMethods()
        {
            var moqPaymentRepository = new Mock<IPaymentRepository>();

            var sut = new PaymentMethodQueryService(moqPaymentRepository.Object);

            var query = new PaymentMethodQuery();

            sut.QueryAsync(query);

            moqPaymentRepository.Verify(
                fake => fake.GetAllPaymentMethodsAsync(),
                Times.Exactly(1));
        }
    }
}
