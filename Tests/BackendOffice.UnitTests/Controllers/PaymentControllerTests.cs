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
using TGS.Apis.BackendOffice.Controllers.Messages.Payment;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;

namespace TGS.Tests.BackendOffice.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable]
    public class PaymentControllerTests : BaseTests
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _controller = new PaymentController(
                _queriesMock.Object,
                _commandsMock.Object);
            _controller.ControllerContext.HttpContext = HttpContextMock.Object;
        }

        private PaymentController _controller;
        private readonly Mock<ITgsQueries<Payment>> _queriesMock = new();

        private readonly Mock<ITgsCommands<Payment>> _commandsMock =
            new();

        [TestCase(true)]
        [TestCase(false)]
        public async Task Can_Get_Payments(bool showAll)
        {
            var request = new PaymentsRequest
            {
                Page = 1,
                Rows = 10,
                SortBy = "updatedOn",
                OrderBy = SortOrder.Descending,
                ShowAll = showAll
            };

            var list = CreatePaymentFixture()
                .CreateMany<Payment>(105)
                .AsQueryable()
                .BuildMock().Object;

            var (skip, take) = GetPage(request);
            var orderedRoles = list.OrderByDescending(p => p.UpdatedOn);

            var expectedRoles = showAll ? orderedRoles.ToArray() : orderedRoles.Skip(skip).Take(take).ToArray();

            _queriesMock.Setup(x => x.GetQueries())
                .Returns(list);

            var response = await _controller.GetPaymentsAsync(request);
            response.Valid.Should().BeTrue();

            for (var i = 0; i < response.Items.Length; i++)
            {
                response.Items[i].Id.Should().Be(expectedRoles[i].Id);
                response.Items[i].Name.Should().Be(expectedRoles[i].Name);
            }

            _queriesMock.Verify(p => p.GetQueries());
        }

        [Test]
        public async Task Cannot_Create_Payment_With_Code_Exists()
        {
            var request = new Fixture().Create<CreatePaymentRequest>();

            _queriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<Payment, bool>>>()))
                .Returns(Task.FromResult(true));

            var response = await _controller.CreateAsync(request);
            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.Payment.CodeExists);
        }

        [Test]
        public async Task Can_Create_Payment()
        {
            var request = new Fixture().Create<CreatePaymentRequest>();

            _queriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<Payment, bool>>>()))
                .Returns(Task.FromResult(false));

            var response = await _controller.CreateAsync(request);
            response.Valid.Should().BeTrue();

            _queriesMock.Verify(s => s.ExistsAsync(It.IsAny<Expression<Func<Payment, bool>>>()));
            _commandsMock.Verify(p => p.CreateAsync(It.Is<Payment>(r =>
                r.Code == request.Code)));
        }

        [Test]
        public async Task Can_Update_Payment()
        {
            var payment = CreatePaymentFixture().Create<Payment>();
            var request = new Fixture().Create<UpdatePaymentRequest>();

            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Payment, bool>>>(),
                    It.IsAny<Func<IQueryable<Payment>, IQueryable<Payment>>>()))
                .Returns(Task.FromResult(payment));

            _queriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<Payment, bool>>>()))
                .Returns(Task.FromResult(false));

            var response = await _controller.UpdateAsync(payment.Id, request);

            response.Valid.Should().BeTrue();
            _queriesMock.Verify(s => s.GetSingleAsync(It.IsAny<Expression<Func<Payment, bool>>>(),
                It.IsAny<Func<IQueryable<Payment>, IQueryable<Payment>>>()));
            _commandsMock.Verify(p => p.UpdateAsync(It.Is<Payment>(r =>
                r.Code == payment.Code)));
        }

        [Test]
        public async Task Can_Update_Payment_Not_Found()
        {
            var payment = CreatePaymentFixture().Create<Payment>();
            var request = new Fixture().Create<UpdatePaymentRequest>();

            _queriesMock.Setup(s => s.GetSingleAsync(
                    It.IsAny<Expression<Func<Payment, bool>>>(),
                    It.IsAny<Func<IQueryable<Payment>,
                        IQueryable<Payment>>>()))
                .Returns(Task.FromResult<Payment>(null));

            _queriesMock.Setup(s =>
                    s.ExistsAsync(It.IsAny<Expression<Func<Payment, bool>>>()))
                .Returns(Task.FromResult(false));

            var response = await _controller.UpdateAsync(payment.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.Payment.NotFound);
            _queriesMock.Verify(s => s.GetSingleAsync(
                It.IsAny<Expression<Func<Payment, bool>>>(),
                It.IsAny<Func<IQueryable<Payment>,
                    IQueryable<Payment>>>()));
            _commandsMock.Verify(p => p.UpdateAsync(It.Is<Payment>(r =>
                r.Code == payment.Code)), Times.Never);
        }

        [Test]
        public async Task Cannot_Update_Payment_With_Code_Exists()
        {
            var payment = CreatePaymentFixture().Create<Payment>();
            var request = new Fixture().Create<UpdatePaymentRequest>();
            request.Code = payment.Code;
            _queriesMock.Setup(s => s.GetSingleAsync(
                    It.IsAny<Expression<Func<Payment, bool>>>(),
                    It.IsAny<Func<IQueryable<Payment>,
                        IQueryable<Payment>>>()))
                .Returns(Task.FromResult(payment));

            _queriesMock.Setup(s =>
                    s.ExistsAsync(It.IsAny<Expression<Func<Payment, bool>>>()))
                .Returns(Task.FromResult(true));

            var response = await _controller.UpdateAsync(payment.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.Payment.CodeExists);
            _queriesMock.Verify(s => s.GetSingleAsync(
                It.IsAny<Expression<Func<Payment, bool>>>(),
                It.IsAny<Func<IQueryable<Payment>,
                    IQueryable<Payment>>>()));
            _commandsMock.Verify(p => p.UpdateAsync(It.Is<Payment>(r =>
                r.Code == payment.Code)), Times.Never);
        }

        private Fixture CreatePaymentFixture()
        {
            var fixture = new Fixture();
            fixture.Customize<Payment>(u => u
                .With(s => s.PaymentMethods, new PaymentMethod[] { })
                .With(s => s.PaymentChannels, new PaymentChannel[] { }));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return fixture;
        }
    }
}