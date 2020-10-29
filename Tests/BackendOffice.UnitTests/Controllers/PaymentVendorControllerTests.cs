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
using TGS.Apis.BackendOffice.Controllers;
using TGS.Apis.BackendOffice.Controllers.Messages.PaymentVendor;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;

namespace TGS.Tests.BackendOffice.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable]
    public class PaymentVendorControllerTests : BaseTests
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _controller = new PaymentVendorController(
                _queriesMock.Object,
                _commandsMock.Object,
                HttpContextAccessorMock.Object);
        }

        private PaymentVendorController _controller;
        private readonly Mock<ITgsQueries<PaymentVendor>> _queriesMock = new Mock<ITgsQueries<PaymentVendor>>();

        private readonly Mock<ITgsCommands<PaymentVendor>> _commandsMock =
            new Mock<ITgsCommands<PaymentVendor>>();

        [TestCase(true)]
        [TestCase(false)]
        public async Task Can_Get_PaymentVendors(bool showAll)
        {
            var request = new PaymentVendorsRequest
            {
                Page = 1,
                Rows = 10,
                SortBy = "updatedOn",
                OrderBy = SortOrder.Descending,
                ShowAll = showAll
            };

            var list = CreatePaymentVendorFixture()
                .CreateMany<PaymentVendor>(105)
                .AsQueryable()
                .BuildMock().Object;

            var (skip, take) = GetPage(request);
            var orderedRoles = list.OrderByDescending(p => p.UpdatedOn);

            var expectedRoles = showAll ? orderedRoles.ToArray() : orderedRoles.Skip(skip).Take(take).ToArray();

            _queriesMock.Setup(x => x.GetQueries())
                .Returns(list);

            var response = await _controller.GetPaymentVendorsAsync(request);
            response.Valid.Should().BeTrue();

            for (var i = 0; i < response.Items.Length; i++)
            {
                response.Items[i].Id.Should().Be(expectedRoles[i].Id);
                response.Items[i].Name.Should().Be(expectedRoles[i].Name);
            }

            _queriesMock.Verify(p => p.GetQueries());
        }

        [Test]
        public async Task Cannot_Create_PaymentVendor_With_Code_Exists()
        {
            var request = new Fixture().Create<CreateUpdatePaymentVendorRequest>();

            _queriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<PaymentVendor, bool>>>()))
                .Returns(Task.FromResult(true));

            var response = await _controller.CreateAsync(request);
            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.PaymentVendor.CodeExists);
        }

        [Test]
        public async Task Can_Create_PaymentVendor()
        {
            var request = new Fixture().Create<CreateUpdatePaymentVendorRequest>();

            _queriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<PaymentVendor, bool>>>()))
                .Returns(Task.FromResult(false));

            var response = await _controller.CreateAsync(request);
            response.Valid.Should().BeTrue();

            _queriesMock.Verify(s => s.ExistsAsync(It.IsAny<Expression<Func<PaymentVendor, bool>>>()));
            _commandsMock.Verify(p => p.CreateAsync(It.Is<PaymentVendor>(r =>
                r.Code == request.Code)));
        }

        [Test]
        public async Task Can_Update_PaymentVendor()
        {
            var payment = CreatePaymentVendorFixture().Create<PaymentVendor>();
            var request = new Fixture().Create<CreateUpdatePaymentVendorRequest>();

            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<PaymentVendor, bool>>>(),
                    It.IsAny<Func<IQueryable<PaymentVendor>, IQueryable<PaymentVendor>>>()))
                .Returns(Task.FromResult(payment));

            _queriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<PaymentVendor, bool>>>()))
                .Returns(Task.FromResult(false));

            var response = await _controller.UpdateAsync(payment.Id, request);

            response.Valid.Should().BeTrue();
            _queriesMock.Verify(s => s.GetSingleAsync(It.IsAny<Expression<Func<PaymentVendor, bool>>>(),
                It.IsAny<Func<IQueryable<PaymentVendor>, IQueryable<PaymentVendor>>>()));
            _commandsMock.Verify(p => p.UpdateAsync(It.Is<PaymentVendor>(r =>
                r.Code == payment.Code)));
        }

        [Test]
        public async Task Can_Update_PaymentVendor_Not_Found()
        {
            var payment = CreatePaymentVendorFixture().Create<PaymentVendor>();
            var request = new Fixture().Create<CreateUpdatePaymentVendorRequest>();

            _queriesMock.Setup(s => s.GetSingleAsync(
                    It.IsAny<Expression<Func<PaymentVendor, bool>>>(),
                    It.IsAny<Func<IQueryable<PaymentVendor>,
                        IQueryable<PaymentVendor>>>()))
                .Returns(Task.FromResult<PaymentVendor>(null));

            _queriesMock.Setup(s =>
                    s.ExistsAsync(It.IsAny<Expression<Func<PaymentVendor, bool>>>()))
                .Returns(Task.FromResult(false));

            var response = await _controller.UpdateAsync(payment.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.PaymentVendor.NotFound);
            _queriesMock.Verify(s => s.GetSingleAsync(
                It.IsAny<Expression<Func<PaymentVendor, bool>>>(),
                It.IsAny<Func<IQueryable<PaymentVendor>,
                    IQueryable<PaymentVendor>>>()));
            _commandsMock.Verify(p => p.UpdateAsync(It.Is<PaymentVendor>(r =>
                r.Code == payment.Code)), Times.Never);
        }

        [Test]
        public async Task Cannot_Update_PaymentVendor_With_Code_Exists()
        {
            var payment = CreatePaymentVendorFixture().Create<PaymentVendor>();
            var request = new Fixture().Create<CreateUpdatePaymentVendorRequest>();
            request.Code = payment.Code;
            _queriesMock.Setup(s => s.GetSingleAsync(
                    It.IsAny<Expression<Func<PaymentVendor, bool>>>(),
                    It.IsAny<Func<IQueryable<PaymentVendor>,
                        IQueryable<PaymentVendor>>>()))
                .Returns(Task.FromResult(payment));

            _queriesMock.Setup(s =>
                    s.ExistsAsync(It.IsAny<Expression<Func<PaymentVendor, bool>>>()))
                .Returns(Task.FromResult(true));

            var response = await _controller.UpdateAsync(payment.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.PaymentVendor.CodeExists);
            _queriesMock.Verify(s => s.GetSingleAsync(
                It.IsAny<Expression<Func<PaymentVendor, bool>>>(),
                It.IsAny<Func<IQueryable<PaymentVendor>,
                    IQueryable<PaymentVendor>>>()));
            _commandsMock.Verify(p => p.UpdateAsync(It.Is<PaymentVendor>(r =>
                r.Code == payment.Code)), Times.Never);
        }

        private Fixture CreatePaymentVendorFixture()
        {
            var fixture = new Fixture();
            fixture.Customize<PaymentVendor>(u => u
                .With(s => s.PaymentMethods, new PaymentMethod[] { })
                .With(s => s.PaymentChannels, new PaymentChannel[] { }));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return fixture;
        }
    }
}