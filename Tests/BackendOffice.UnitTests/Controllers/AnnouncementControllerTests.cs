using System;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers;
using TGS.Apis.BackendOffice.Controllers.Messages.Announcement;
using TGS.Apis.BackendOffice.Controllers.Messages.Payment;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;
using TGS.Core.Models.Enums;

namespace TGS.Tests.BackendOffice.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable]
    public class AnnouncementControllerTests : BaseTests
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _controller = new AnnouncementController(
                _queriesMock.Object,
                _commandsMock.Object,
                HttpContextAccessorMock.Object);
        }

        private AnnouncementController _controller;
        private readonly Mock<ITgsQueries<Announcement>> _queriesMock = new Mock<ITgsQueries<Announcement>>();
        private readonly Mock<ITgsCommands<Announcement>> _commandsMock = new Mock<ITgsCommands<Announcement>>();

        [TestCase(true, new[] {AnnounceStatus.Effective, AnnounceStatus.Invalid})]
        [TestCase(false, new[] {AnnounceStatus.Expired})]
        public async Task Can_Get_Announcements(bool showAll, AnnounceStatus[] status)
        {
            var request = new GetAnnouncementsRequest
            {
                Page = 1,
                Rows = 10,
                SortBy = "UpdatedOn",
                OrderBy = SortOrder.Descending,
                ShowAll = showAll,
                Status = status
            };

            var list = CreateAnnouncementFixture()
                .CreateMany<Announcement>(105)
                .AsQueryable()
                .BuildMock().Object;

            var (skip, take) = GetPage(request);
            var orderedRoles = list.Where(request.Status.Contains(AnnounceStatus.Expired),
                    p => DateTimeOffset.Now > p.To)
                .Where(!request.Status.Contains(AnnounceStatus.Expired),
                    p => request.Status == null || request.Status.Contains(p.Status))
                .OrderByDescending(p => p.UpdatedOn);

            var expectedRoles = showAll ? orderedRoles.ToArray() : orderedRoles.Skip(skip).Take(take).ToArray();

            _queriesMock.Setup(x => x.GetQueries())
                .Returns(list);

            var response = await _controller.GetListAsync(request);
            response.Valid.Should().BeTrue();

            for (var i = 0; i < response.Items.Length; i++)
            {
                response.Items[i].Id.Should().Be(expectedRoles[i].Id);
                response.Items[i].Subject.Should().Be(expectedRoles[i].Subject);
                response.Items[i].Content.Should().Be(expectedRoles[i].Content);
                response.Items[i].Remarks.Should().Be(expectedRoles[i].Remarks);
                response.Items[i].CreatedByUsername.Should().Be(expectedRoles[i].CreatedByUsername);
                response.Items[i].VerifiedByUsername.Should().Be(expectedRoles[i].VerifiedByUsername);
                response.Items[i].From.Should().Be(expectedRoles[i].From);
                response.Items[i].To.Should().Be(expectedRoles[i].To);
                response.Items[i].UpdatedOn.Should().Be(expectedRoles[i].UpdatedOn);
                response.Items[i].Status.Should().Be(DateTimeOffset.Now > expectedRoles[i].To
                    ? AnnounceStatus.Expired.ToString()
                    : expectedRoles[i].Status.ToString());
            }

            _queriesMock.Verify(p => p.GetQueries());
        }

        [Test]
        public async Task Can_Create_Announcement()
        {
            var request = new Fixture().Create<AnnouncementRequest>();

            _queriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<Announcement, bool>>>()))
                .Returns(Task.FromResult(false));

            var response = await _controller.CreateAsync(request);
            response.Valid.Should().BeTrue();

            _commandsMock.Verify(p => p.CreateAsync(It.Is<Announcement>(d =>
                d.Subject == request.Subject &&
                d.Content == request.Content &&
                d.From == request.From &&
                d.To == request.To &&
                d.Status == AnnounceStatus.InVerify)));
        }

        [Test]
        public async Task Can_Update_Announcement()
        {
            var announcement = CreateAnnouncementFixture().Create<Announcement>();
            announcement.Status = AnnounceStatus.Invalid;

            var request = new Fixture().Create<AnnouncementRequest>();

            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Announcement, bool>>>(),
                    It.IsAny<Func<IQueryable<Announcement>, IQueryable<Announcement>>>()))
                .Returns(Task.FromResult(announcement));

            _queriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<Announcement, bool>>>()))
                .Returns(Task.FromResult(false));

            var response = await _controller.UpdateAsync(announcement.Id, request);

            response.Valid.Should().BeTrue();
            _queriesMock.Verify(s => s.GetSingleAsync(It.IsAny<Expression<Func<Announcement, bool>>>(),
                It.IsAny<Func<IQueryable<Announcement>, IQueryable<Announcement>>>()));

            _commandsMock.Verify(p => p.UpdateAsync(It.Is<Announcement>(d =>
                d.Subject == request.Subject &&
                d.Content == request.Content &&
                d.From == request.From &&
                d.To == request.To &&
                d.Status == AnnounceStatus.InVerify)));
        }

        [Test]
        public async Task Can_Update_Announcement_Not_Found()
        {
            var announcement = CreateAnnouncementFixture().Create<Announcement>();
            announcement.Status = AnnounceStatus.InVerify;
            var request = new Fixture().Create<AnnouncementRequest>();

            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Announcement, bool>>>(),
                    It.IsAny<Func<IQueryable<Announcement>, IQueryable<Announcement>>>()))
                .Returns(Task.FromResult<Announcement>(null));

            _queriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<Announcement, bool>>>()))
                .Returns(Task.FromResult(false));

            var response = await _controller.UpdateAsync(announcement.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.Announcement.NotFound);
            _queriesMock.Verify(s => s.GetSingleAsync(It.IsAny<Expression<Func<Announcement, bool>>>(),
                It.IsAny<Func<IQueryable<Announcement>, IQueryable<Announcement>>>()));
            _commandsMock.Verify(p => p.UpdateAsync(It.Is<Announcement>(d =>
                d.Subject == request.Subject &&
                d.Content == request.Content &&
                d.From == request.From &&
                d.To == request.To &&
                d.Status == AnnounceStatus.InVerify)), Times.Never);
        }

        [Test]
        public async Task Cannot_Update_Announcement_With_Invalid_Status()
        {
            var announcement = CreateAnnouncementFixture().Create<Announcement>();
            announcement.Status = AnnounceStatus.InVerify;
            var request = new Fixture().Create<AnnouncementRequest>();
            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Announcement, bool>>>(),
                    It.IsAny<Func<IQueryable<Announcement>, IQueryable<Announcement>>>()))
                .Returns(Task.FromResult(announcement));

            _queriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<Announcement, bool>>>()))
                .Returns(Task.FromResult(true));

            var response = await _controller.UpdateAsync(announcement.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.Announcement.CannotUpdateDueToInvalidStatus);
            _queriesMock.Verify(s => s.GetSingleAsync(It.IsAny<Expression<Func<Announcement, bool>>>(),
                It.IsAny<Func<IQueryable<Announcement>, IQueryable<Announcement>>>()));
            _commandsMock.Verify(p => p.UpdateAsync(It.Is<Announcement>(d =>
                d.Subject == request.Subject &&
                d.Content == request.Content &&
                d.From == request.From &&
                d.To == request.To &&
                d.Status == AnnounceStatus.InVerify)), Times.Never);
        }

        [TestCase("pass", AnnounceStatus.InVerify, AnnounceStatus.Effective)]
        [TestCase("reject", AnnounceStatus.InVerify, AnnounceStatus.Rejected)]
        public async Task Can_Verify_Announcement(string type, AnnounceStatus allowedStatus,
            AnnounceStatus verifiedStatus)
        {
            var announcement = CreateAnnouncementFixture().Create<Announcement>();
            announcement.Status = allowedStatus;
            var announcementId = Guid.NewGuid();

            var request = new Fixture().Create<VerifyRequest>();

            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Announcement, bool>>>(),
                    It.IsAny<Func<IQueryable<Announcement>, IQueryable<Announcement>>>()))
                .Returns(Task.FromResult(announcement));

            var response = await _controller.VerifyAsync(announcementId, type, request);

            response.Valid.Should().BeTrue();
            _queriesMock.Verify(s => s.GetSingleAsync(It.IsAny<Expression<Func<Announcement, bool>>>(),
                It.IsAny<Func<IQueryable<Announcement>, IQueryable<Announcement>>>()));

            _commandsMock.Verify(p => p.UpdateAsync(It.Is<Announcement>(d =>
                d.UpdatedByUsername == "admin.enabled" &&
                d.Status == verifiedStatus &&
                d.Remarks == request.Remarks)));
        }


        [TestCase("invalid", AnnounceStatus.Effective, AnnounceStatus.Invalid)]
        public async Task Can_Invalid_Announcement(string type, AnnounceStatus allowedStatus,
            AnnounceStatus verifiedStatus)
        {
            var announcement = CreateAnnouncementFixture().Create<Announcement>();
            announcement.Status = allowedStatus;
            var announcementId = Guid.NewGuid();

            var request = new Fixture().Create<VerifyRequest>();

            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Announcement, bool>>>(),
                    It.IsAny<Func<IQueryable<Announcement>, IQueryable<Announcement>>>()))
                .Returns(Task.FromResult(announcement));

            var response = await _controller.VerifyAsync(announcementId, type, request);

            response.Valid.Should().BeTrue();
            _queriesMock.Verify(s => s.GetSingleAsync(It.IsAny<Expression<Func<Announcement, bool>>>(),
                It.IsAny<Func<IQueryable<Announcement>, IQueryable<Announcement>>>()));

            _commandsMock.Verify(p => p.UpdateAsync(It.Is<Announcement>(d =>
                d.UpdatedByUsername == "admin.enabled" &&
                d.Status == verifiedStatus)));
        }

        [Test]
        public async Task Cannot_Verify_Announcement_With_Invalid_Path()
        {
            var announcement = CreateAnnouncementFixture().Create<Announcement>();
            var announcementId = Guid.NewGuid();

            var request = new Fixture().Create<VerifyRequest>();

            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Announcement, bool>>>(),
                    It.IsAny<Func<IQueryable<Announcement>, IQueryable<Announcement>>>()))
                .Returns(Task.FromResult(announcement));

            var response = await _controller.VerifyAsync(announcementId, "pass2", request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.Announcement.InvalidUrl);
        }

        [Test]
        public async Task Cannot_Verify_Announcement_Not_Found()
        {
            var announcementId = Guid.NewGuid();

            var request = new Fixture().Create<VerifyRequest>();

            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Announcement, bool>>>(),
                    It.IsAny<Func<IQueryable<Announcement>, IQueryable<Announcement>>>()))
                .Returns(Task.FromResult<Announcement>(null));

            var response = await _controller.VerifyAsync(announcementId, "pass", request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.Announcement.NotFound);

            _queriesMock.Verify(s => s.GetSingleAsync(It.IsAny<Expression<Func<Announcement, bool>>>(),
                It.IsAny<Func<IQueryable<Announcement>, IQueryable<Announcement>>>()));

            _commandsMock.Verify(p => p.UpdateAsync(It.Is<Announcement>(d =>
                d.UpdatedByUsername == "admin.enabled" &&
                d.Remarks == request.Remarks)), Times.Never);
        }


        [TestCase("pass", AnnounceStatus.Effective, "Only announcement in verify can be passed.")]
        [TestCase("pass", AnnounceStatus.Invalid, "Only announcement in verify can be passed.")]
        [TestCase("pass", AnnounceStatus.Expired, "Only announcement in verify can be passed.")]
        [TestCase("pass", AnnounceStatus.Rejected, "Only announcement in verify can be passed.")]
        [TestCase("reject", AnnounceStatus.Effective, "Only announcement in verify can be rejected.")]
        [TestCase("reject", AnnounceStatus.Invalid, "Only announcement in verify can be rejected.")]
        [TestCase("reject", AnnounceStatus.Expired, "Only announcement in verify can be rejected.")]
        [TestCase("reject", AnnounceStatus.Rejected, "Only announcement in verify can be rejected.")]
        [TestCase("invalid", AnnounceStatus.InVerify, "Only effective announcement can be invalided.")]
        [TestCase("invalid", AnnounceStatus.Invalid, "Only effective announcement can be invalided.")]
        [TestCase("invalid", AnnounceStatus.Expired, "Only effective announcement can be invalided.")]
        [TestCase("invalid", AnnounceStatus.Rejected, "Only effective announcement can be invalided.")]
        public async Task Cannot_Verify_Announcement_With_Invalid_Status(string type, AnnounceStatus invalidStatus,
            string message)
        {
            var announcementId = Guid.NewGuid();
            var announcement = CreateAnnouncementFixture().Create<Announcement>();
            announcement.Status = invalidStatus;

            var request = new Fixture().Create<VerifyRequest>();

            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Announcement, bool>>>(),
                    It.IsAny<Func<IQueryable<Announcement>, IQueryable<Announcement>>>()))
                .Returns(Task.FromResult(announcement));

            var response = await _controller.VerifyAsync(announcementId, type, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(message);
        }

        [Test]
        public async Task Can_Change_Announcement_Order()
        {
            var list = CreateAnnouncementFixture()
                .CreateMany<Announcement>(105)
                .AsQueryable()
                .BuildMock().Object;

            var request = new ChangeOrderRequest
            {
                Orders = new[]
                {
                    new ChangeOrderModel
                    {
                        Id = (await list.FirstOrDefaultAsync()).Id,
                        DisplayOrder = 1
                    },
                    new ChangeOrderModel
                    {
                        Id = (await list.LastOrDefaultAsync()).Id,
                        DisplayOrder = 2
                    }
                }
            };

            _queriesMock.Setup(x => x.GetQueries())
                .Returns(list);

            var response = await _controller.ChangeOrderAsync(request);

            response.Valid.Should().BeTrue();

            _queriesMock.Verify(p => p.GetQueries());

            _commandsMock.Verify(p => p.BatchInsertOrUpdateAsync(It.Is<Announcement[]>(d =>
                d.Length == request.Orders.Length)));
        }

        [TestCase(AnnounceStatus.Effective)]
        [TestCase(AnnounceStatus.Invalid)]
        public async Task Can_Change_Announcement_Status(AnnounceStatus status)
        {
            var announcement = CreateAnnouncementFixture()
                .Create<Announcement>();

            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Announcement, bool>>>(),
                    It.IsAny<Func<IQueryable<Announcement>, IQueryable<Announcement>>>()))
                .Returns(Task.FromResult(announcement));

            var response = await _controller.ChangeStatusAsync(announcement.Id, status);

            response.Valid.Should().BeTrue();

            _queriesMock.Verify(s => s.GetSingleAsync(It.IsAny<Expression<Func<Announcement, bool>>>(),
                It.IsAny<Func<IQueryable<Announcement>, IQueryable<Announcement>>>()));

            _commandsMock.Verify(p => p.UpdateAsync(It.Is<Announcement>(d =>
                d.Status == status)));
        }

        [Test]
        public async Task Cannot_Change_Announcement_Status_Not_Found()
        {
            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Announcement, bool>>>(),
                    It.IsAny<Func<IQueryable<Announcement>, IQueryable<Announcement>>>()))
                .Returns(Task.FromResult<Announcement>(null));

            var response = await _controller.ChangeStatusAsync(Guid.NewGuid(), AnnounceStatus.Invalid);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.Announcement.NotFound);
        }


        private Fixture CreateAnnouncementFixture()
        {
            var fixture = new Fixture();

            return fixture;
        }
    }
}