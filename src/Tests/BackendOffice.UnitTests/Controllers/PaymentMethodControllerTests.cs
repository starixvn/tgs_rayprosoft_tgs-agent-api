using System;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers.BrandPayment;
using TGS.Apis.BackendOffice.Controllers.Messages.PaymentMethod;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Tests.BackendOffice.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable]
    public class PaymentMethodControllerTests : BaseTests
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _controller = new PaymentMethodController(
                _queriesMock.Object,
                _commandsMock.Object);
            _controller.ControllerContext.HttpContext = HttpContextMock.Object;
        }

        private PaymentMethodController _controller;
        private readonly Mock<ITgsQueries<PaymentMethod>> _queriesMock = new();

        private readonly Mock<ITgsCommands<PaymentMethod>> _commandsMock =
            new();

        [TestCase(true)]
        [TestCase(false)]
        public async Task Can_Get_PaymentMethods(bool showAll)
        {
            var request = new PaymentMethodsRequest
            {
                Page = 1,
                Rows = 10,
                SortBy = "updatedOn",
                OrderBy = SortOrder.Descending,
                ShowAll = showAll
            };

            var list = CreatePaymentMethodFixture()
                .CreateMany<PaymentMethod>(105)
                .AsQueryable()
                .BuildMock().Object;

            var (skip, take) = GetPage(request);
            Assert.Fail();
            //var orderedRoles = list.OrderByDescending(p => p.UpdatedOn);

            //var expectedRoles = showAll ? orderedRoles.ToArray() : orderedRoles.Skip(skip).Take(take).ToArray();

            //_queriesMock.Setup(x => x.GetQueries())
            //    .Returns(list);

            //var response = await _controller.GetPaymentMethodsAsync(request);
            //response.Valid.Should().BeTrue();

            //for (var i = 0; i < response.Items.Length; i++)
            //{
            //    response.Items[i].Id.Should().Be(expectedRoles[i].Id);
            //    response.Items[i].Name.Should().Be(expectedRoles[i].Name);
            //}

            //_queriesMock.Verify(p => p.GetQueries());
        }

        [Test]
        public async Task Cannot_Create_PaymentMethod_With_Code_Exists()
        {
            var request = new Fixture().Create<CreatePaymentMethodRequest>();

            _queriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<PaymentMethod, bool>>>()))
                .Returns(Task.FromResult(true));

            Assert.Fail();
            //var response = await _controller.CreateAsync(request);
            //response.Valid.Should().BeFalse();
            //response.Message.Should().Be(Apis.BackendOffice.Resources.PaymentMethod.CodeExists);
        }

        [Test]
        public async Task Can_Create_PaymentMethod()
        {
            var request = new Fixture().Create<CreatePaymentMethodRequest>();

            _queriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<PaymentMethod, bool>>>()))
                .Returns(Task.FromResult(false));

            Assert.Fail();
            //var response = await _controller.CreateAsync(request);
            //response.Valid.Should().BeTrue();

            //_queriesMock.Verify(s => s.ExistsAsync(It.IsAny<Expression<Func<PaymentMethod, bool>>>()));
            //_commandsMock.Verify(p => p.CreateAsync(It.Is<PaymentMethod>(r =>
            //    r.Code == request.Code)));
        }

        [Test]
        public async Task Can_Update_PaymentMethod()
        {
            var payment = CreatePaymentMethodFixture().Create<PaymentMethod>();
            var request = new Fixture().Create<UpdatePaymentMethodRequest>();

            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<PaymentMethod, bool>>>(),
                    It.IsAny<Func<IQueryable<PaymentMethod>, IQueryable<PaymentMethod>>>()))
                .Returns(Task.FromResult(payment));

            _queriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<PaymentMethod, bool>>>()))
                .Returns(Task.FromResult(false));

            Assert.Fail();
            //var response = await _controller.UpdateAsync(payment.Id, request);

            //response.Valid.Should().BeTrue();
            //_queriesMock.Verify(s => s.GetSingleAsync(It.IsAny<Expression<Func<PaymentMethod, bool>>>(),
            //    It.IsAny<Func<IQueryable<PaymentMethod>, IQueryable<PaymentMethod>>>()));
            //_commandsMock.Verify(p => p.UpdateAsync(It.Is<PaymentMethod>(r =>
            //    r.Code == payment.Code)));
        }

        [Test]
        public async Task Can_Update_PaymentMethod_Not_Found()
        {
            var payment = CreatePaymentMethodFixture().Create<PaymentMethod>();
            var request = new Fixture().Create<UpdatePaymentMethodRequest>();

            _queriesMock.Setup(s => s.GetSingleAsync(
                    It.IsAny<Expression<Func<PaymentMethod, bool>>>(),
                    It.IsAny<Func<IQueryable<PaymentMethod>,
                        IQueryable<PaymentMethod>>>()))
                .Returns(Task.FromResult<PaymentMethod>(null));

            _queriesMock.Setup(s =>
                    s.ExistsAsync(It.IsAny<Expression<Func<PaymentMethod, bool>>>()))
                .Returns(Task.FromResult(false));

            Assert.Fail();
            //var response = await _controller.UpdateAsync(payment.Id, request);

            //response.Valid.Should().BeFalse();
            //response.Message.Should().Be(Apis.BackendOffice.Resources.PaymentMethod.NotFound);
            //_queriesMock.Verify(s => s.GetSingleAsync(
            //    It.IsAny<Expression<Func<PaymentMethod, bool>>>(),
            //    It.IsAny<Func<IQueryable<PaymentMethod>,
            //        IQueryable<PaymentMethod>>>()));
            //_commandsMock.Verify(p => p.UpdateAsync(It.Is<PaymentMethod>(r =>
            //    r.Code == payment.Code)), Times.Never);
        }

        private Fixture CreatePaymentMethodFixture()
        {
            var fixture = new Fixture();
            //fixture.Customize<PaymentMethod>(u => u
            //    .With(s => s.PaymentChannels, new PaymentChannel[] { }));
            //fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            Assert.Fail();
            return fixture;
        }
    }
}