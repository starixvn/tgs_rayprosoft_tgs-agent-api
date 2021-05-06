using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TGS.Services.BackOffice.Application.Queries.BrandPayment;
using TGS.Services.BackOffice.Application.Queries.PaymentChannel;
using TGS.Services.Payment;

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
