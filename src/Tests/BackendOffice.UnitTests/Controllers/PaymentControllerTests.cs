using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers.BrandPayment;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Tgs;
using PaymentData = TGS.Core.DataServices.Tgs.Models.Payment;

namespace TGS.Tests.BackendOffice.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable]
    public class PaymentControllerTests
    {
        [Test]
        public void Can_Get_All_Payments()
        {
            var moqQueries = new Mock<ITgsQueries<PaymentData>>();
            var moqCommands = new Mock<ITgsCommands<PaymentData>>();

            var sut = new PaymentController(
                moqQueries.Object,
                moqCommands.Object);

            moqQueries.Setup(x => x.GetQueries())
                .Returns(new[] {
                    new PaymentData{
                        Id = Guid.NewGuid(),
                        Code = "code1",
                        Name = "name1",
                        Status = ActivateStatus.Active
                    }
                }.AsQueryable());

            var response = sut.GetAllPayments();

            moqQueries.Verify(p => p.GetQueries(), Times.Exactly(1));
            Assert.IsTrue(response.Data.Count() == 1);
        }
    }
}
