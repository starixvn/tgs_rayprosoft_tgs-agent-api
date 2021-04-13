using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers.BrandPayment;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Tests.BackendOffice.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable]
    public class PaymentMethodControllerTests : BaseTests
    {
        [Test]
        public void Can_Get_All_PaymentMethods()
        {
            var moqQueries = new Mock<ITgsQueries<PaymentMethod>>();
            var moqCommands = new Mock<ITgsCommands<PaymentMethod>>();

            var sut = new PaymentMethodController(
                moqQueries.Object,
                moqCommands.Object);

            moqQueries.Setup(x => x.GetQueries())
                .Returns(new[] {
                    new PaymentMethod{
                        Id = Guid.NewGuid(),
                        Code = "code1"
                    }
                }.AsQueryable());

            var response = sut.GetAllPaymentMethods();

            moqQueries.Verify(p => p.GetQueries());
            Assert.IsTrue(response.Data.Count() == 1);
        }
    }
}
