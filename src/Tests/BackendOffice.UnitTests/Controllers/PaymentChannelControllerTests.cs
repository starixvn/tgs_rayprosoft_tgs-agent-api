using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers.BrandPayment;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Tests.BackendOffice.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable]
    public class PaymentChannelControllerTests
    {
        [Test]
        public void Can_Get_PaymentChannels()
        {
            var moqQueries = new Mock<ITgsQueries<PaymentChannel>>();
            var moqCommands = new Mock<ITgsCommands<PaymentChannel>>();

            var sut = new PaymentChannelController(
                moqQueries.Object,
                moqCommands.Object);

            moqQueries.Setup(x => x.GetQueries())
                .Returns(new[] {
                    new PaymentChannel{
                        Id = Guid.NewGuid(),
                        Code = "code1"
                    }
                }.AsQueryable());

            var response = sut.GetAllPaymentChannels();

            moqQueries.Verify(p => p.GetQueries());
            Assert.IsTrue(response.Data.Count() == 1);
        }
    }
}
