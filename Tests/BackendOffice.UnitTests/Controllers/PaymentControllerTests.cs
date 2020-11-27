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
using TGS.Apis.BackendOffice.Controllers.Messages.Payment;
using TGS.Apis.BackendOffice.Controllers.Payment;
using TGS.Apis.BackendOffice.DataService.Services;
using TGS.Apis.BackendOffice.Services;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;
using TGS.Core.Models.Enums;

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
                _commandsMock.Object,
                _auditLogQueriesMock.Object,
                _playerQueriesMock.Object,
                _uploadServiceMock.Object,
                _paymentServiceMock.Object,
                HttpContextAccessorMock.Object);
        }

        private PaymentController _controller;
        private readonly Mock<IPaymentService> _paymentServiceMock = new Mock<IPaymentService>();
        private readonly Mock<ITgsQueries<Payment>> _queriesMock = new Mock<ITgsQueries<Payment>>();

        private readonly Mock<ITgsQueries<PaymentAuditLog>> _auditLogQueriesMock =
            new Mock<ITgsQueries<PaymentAuditLog>>();

        private readonly Mock<ITgsCommands<Payment>> _commandsMock =
            new Mock<ITgsCommands<Payment>>();

        private readonly Mock<ITgsQueries<Player>> _playerQueriesMock =
            new Mock<ITgsQueries<Player>>();

        private readonly Mock<IUploadService> _uploadServiceMock = new Mock<IUploadService>();

        [TestCase(true)]
        [TestCase(false)]
        public async Task Can_Get_Payments(bool showAll)
        {
            var request = new GetPaymentsRequest
            {
                PaymentStatus = PaymentStatus.Enabled,
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
            var orderedRoles = list.Where(x => x.PaymentStatus == PaymentStatus.Enabled)
                .OrderByDescending(p => p.UpdatedOn);

            var expectedRoles = showAll ? orderedRoles.ToArray() : orderedRoles.Skip(skip).Take(take).ToArray();

            _queriesMock.Setup(x => x.GetQueries())
                .Returns(list);

            var response = await _controller.GetPaymentsAsync(request);
            response.Valid.Should().BeTrue();

            for (var i = 0; i < response.Items.Length; i++)
            {
                response.Items[i].Id.Should().Be(expectedRoles[i].Id);
                response.Items[i].ChannelName.Should().Be(expectedRoles[i].ChannelName);
            }

            _queriesMock.Verify(p => p.GetQueries());
        }

        [Test]
        public async Task Cannot_Create_Payment_With_Name_Exists()
        {
            var request = new Fixture().Create<CreatePaymentRequest>();

            _queriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<Payment, bool>>>()))
                .Returns(Task.FromResult(true));

            var response = await _controller.CreatePaymentAsync(request);
            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.Payment.NameExists);
        }

        [Test]
        public async Task Cannot_Create_Payment_With_Whitelist_Same_As_Blacklist()
        {
            var request = new Fixture().Create<CreatePaymentRequest>();
            request.Whitelist[0] = request.Blacklist[0];
            var response = await _controller.CreatePaymentAsync(request);
            response.Valid.Should().BeFalse();
            response.DuplicatePlayers[0].Should().Be(request.Whitelist[0]);
            response.Message.Should().Be(Apis.BackendOffice.Resources.Payment.WhiteSameAsBlack);
        }

        [Test]
        public async Task Can_Create_Payment()
        {
            var request = new Fixture().Create<CreatePaymentRequest>();

            _queriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<Payment, bool>>>()))
                .Returns(Task.FromResult(false));

            var response = await _controller.CreatePaymentAsync(request);
            response.Valid.Should().BeTrue();

            _queriesMock.Verify(s => s.ExistsAsync(It.IsAny<Expression<Func<Payment, bool>>>()));
            _paymentServiceMock.Verify(p => p.CreatePaymentAsync(It.Is<CreatePaymentRequest>(r =>
                    r.ChannelName == request.ChannelName),
                It.Is<string>(r => r == Username)));
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

            var response = await _controller.UpdatePaymentAsync(payment.Id, request);

            response.Valid.Should().BeTrue();
            _queriesMock.Verify(s => s.GetSingleAsync(It.IsAny<Expression<Func<Payment, bool>>>(),
                It.IsAny<Func<IQueryable<Payment>, IQueryable<Payment>>>()));
            _paymentServiceMock.Verify(p => p.UpdatePaymentAsync(It.Is<Payment>(r =>
                    r.ChannelName == payment.ChannelName),
                It.IsAny<UpdatePaymentRequest>(),
                It.Is<string>(r => r == Username)));
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

            var response = await _controller.UpdatePaymentAsync(payment.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.Payment.NotFound);
            _queriesMock.Verify(s => s.GetSingleAsync(
                It.IsAny<Expression<Func<Payment, bool>>>(),
                It.IsAny<Func<IQueryable<Payment>,
                    IQueryable<Payment>>>()));
            _paymentServiceMock.Verify(p => p.UpdatePaymentAsync(It.Is<Payment>(r =>
                    r.ChannelName == payment.ChannelName),
                It.IsAny<UpdatePaymentRequest>(),
                It.Is<string>(r => r == Username)), Times.Never);
        }

        [Test]
        public async Task Cannot_Update_Payment_With_Name_Exists()
        {
            var payment = CreatePaymentFixture().Create<Payment>();
            var request = new Fixture().Create<UpdatePaymentRequest>();
            request.ChannelName = payment.ChannelName;
            _queriesMock.Setup(s => s.GetSingleAsync(
                    It.IsAny<Expression<Func<Payment, bool>>>(),
                    It.IsAny<Func<IQueryable<Payment>,
                        IQueryable<Payment>>>()))
                .Returns(Task.FromResult(payment));

            _queriesMock.Setup(s =>
                    s.ExistsAsync(It.IsAny<Expression<Func<Payment, bool>>>()))
                .Returns(Task.FromResult(true));

            var response = await _controller.UpdatePaymentAsync(payment.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.Payment.NameExists);
            _queriesMock.Verify(s => s.GetSingleAsync(
                It.IsAny<Expression<Func<Payment, bool>>>(),
                It.IsAny<Func<IQueryable<Payment>,
                    IQueryable<Payment>>>()));
            _paymentServiceMock.Verify(p => p.UpdatePaymentAsync(It.Is<Payment>(r =>
                    r.ChannelName == payment.ChannelName),
                It.IsAny<UpdatePaymentRequest>(),
                It.Is<string>(r => r == Username)), Times.Never);
        }

        private Fixture CreatePaymentFixture()
        {
            var fixture = new Fixture();
            fixture.Customize<Payment>(u => u
                .With(s => s.PaymentPlayerList, new PaymentPlayerList[] { })
                .With(s => s.VipLevels, new PaymentVipLevel[] { }));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return fixture;
        }
    }
}