using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.StationLetter;
using TGS.Apis.BackendOffice.DataService.Repositories;
using TGS.Apis.BackendOffice.Services;
using TGS.Core.DataServices.Models;
using TGS.Core.Models.Enums;
using Resources = TGS.Apis.BackendOffice.Resources;

namespace BackendOffice.UnitTests.Controllers
{
    [TestFixture, Parallelizable]
    public class StationLetterControllerTests : BaseTests
    {
        private StationLetterController _controller;

        private readonly Mock<ITgsQueries<Player>> _playerQueriesMock = new Mock<ITgsQueries<Player>>();
        private readonly Mock<ITgsQueries<StationLetter>> _queriesMock = new Mock<ITgsQueries<StationLetter>>();
        private readonly Mock<ITgsCommands<StationLetter>> _commandsMock = new Mock<ITgsCommands<StationLetter>>();
        private readonly Mock<ITgsCommands<StationLetterPlayer>> _playerCommandsMock = new Mock<ITgsCommands<StationLetterPlayer>>();
        private readonly Mock<IUploadService> _uploadServiceMock = new Mock<IUploadService>();

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _controller = new StationLetterController(
                _queriesMock.Object,
                _playerQueriesMock.Object,
                _commandsMock.Object,
                _playerCommandsMock.Object,
                _uploadServiceMock.Object,
                HttpContextAccessorMock.Object);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Can_Get_List(bool showAll)
        {
            var request = new PageableRequest
            {
                Page = 1,
                Rows = 10,
                SortBy = "CreatedOn",
                OrderBy = SortOrder.Descending,
                ShowAll = showAll
            };

            var list = CreateStationLetterFixture()
                .CreateMany<StationLetter>(105)
                .AsQueryable()
                .BuildMock().Object;

            var (skip, take) = GetPage(request);
            var orderedPlayers = list.OrderByDescending(p => p.CreatedOn);

            var expectedPlayers = showAll ?
                orderedPlayers.ToArray() :
                orderedPlayers.Skip(skip).Take(take).ToArray();

            _queriesMock.Setup(x => x.GetQueries())
                .Returns(list);

            var response = await _controller.GetListAsync(request);
            response.Valid.Should().BeTrue();

            for (var i = 0; i < response.Items.Length; i++)
            {
                response.Items[i].Id.Should().Be(expectedPlayers[i].Id);
                response.Items[i].Subject.Should().Be(expectedPlayers[i].Subject);
                response.Items[i].Content.Should().Be(expectedPlayers[i].Content);
                response.Items[i].Status.Should().Be(expectedPlayers[i].Status.ToString());
                response.Items[i].Remarks.Should().Be(expectedPlayers[i].Remarks);

                response.Items[i].CreatedByUsername.Should().Be(expectedPlayers[i].CreatedByUsername);
                response.Items[i].VerifiedByUsername.Should().Be(expectedPlayers[i].VerifiedByUsername);
                response.Items[i].CreatedOn.Should().Be(expectedPlayers[i].CreatedOn);
            }

            _queriesMock.Verify(p => p.GetQueries());
        }

        [Test]
        public async Task Can_Create()
        {
            var players = CreatePlayerFixture()
                .CreateMany<Player>(200)
                .AsQueryable()
                .BuildMock().Object;

            var request = new Fixture().Create<StationLetterRequest>();
            request.AssignedAllPlayers = false;
            request.PlayerUsernames = players
                .Where(p => p.Status == PlayerStatus.Enabled)
                .Select(p => p.Username).ToArray();

            _playerQueriesMock.Setup(p => p.GetQueries())
                .Returns(players);

            var response = await _controller.CreateAsync(request);
            response.Valid.Should().BeTrue();
            _commandsMock.Verify(p => p.CreateAsync(It.Is<StationLetter>(s =>
                  s.Subject == request.Subject &&
                  s.Content == request.Content &&
                  s.AssignedAllPlayers == request.AssignedAllPlayers)));

            _playerCommandsMock.Verify(p => p.BatchInsert(It.Is<StationLetterPlayer[]>(s =>
                s.Length == request.PlayerUsernames.Length)));
        }

        [Test]
        public async Task Can_Update()
        {
            var stationLetterId = Guid.NewGuid();
            var request = new Fixture().Create<StationLetterRequest>();
            var stationLetter = CreateStationLetterFixture().Create<StationLetter>();

            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<StationLetter, bool>>>(), It.IsAny<Func<IQueryable<StationLetter>, IQueryable<StationLetter>>>()))
                .Returns(Task.FromResult(stationLetter));

            var response = await _controller.UpdateAsync(stationLetterId, request);
            response.Valid.Should().BeTrue();
            _commandsMock.Verify(p => p.UpdateAsync(It.Is<StationLetter>(s =>
                s.Subject == request.Subject &&
                s.Content == request.Content)));
        }

        [Test]
        public async Task Can_Update_Not_Found()
        {
            var stationLetterId = Guid.NewGuid();
            var request = new Fixture().Create<StationLetterRequest>();

            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<StationLetter, bool>>>(), It.IsAny<Func<IQueryable<StationLetter>, IQueryable<StationLetter>>>()))
                .Returns(Task.FromResult<StationLetter>(null));

            var response = await _controller.UpdateAsync(stationLetterId, request);
            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.StationLetter.NotFound);
            _commandsMock.Verify(p => p.UpdateAsync(It.Is<StationLetter>(s =>
                s.Subject == request.Subject &&
                s.Content == request.Content)), Times.Never);
        }

        [TestCase("pass", StationLetterStatus.Published)]
        [TestCase("reject", StationLetterStatus.Rejected)]
        public async Task Can_Verify(string type, StationLetterStatus targetStatus)
        {
            var request = new Fixture().Create<VerifyRequest>();
            var stationLetter = CreateStationLetterFixture().Create<StationLetter>();
            stationLetter.Status = StationLetterStatus.InVerify;

            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<StationLetter, bool>>>(), It.IsAny<Func<IQueryable<StationLetter>, IQueryable<StationLetter>>>()))
                .Returns(Task.FromResult(stationLetter));

            var response = await _controller.VerifyAsync(stationLetter.Id, type, request);
            response.Valid.Should().BeTrue();
            _commandsMock.Verify(p => p.UpdateAsync(It.Is<StationLetter>(s =>
                s.Status == targetStatus &&
                s.UpdatedByUsername == Username &&
                s.VerifiedByUsername == Username &&
                s.Remarks == request.Remarks)));
        }

        [Test]
        public async Task Cannot_Verify_With_Invalid_Url()
        {
            var request = new Fixture().Create<VerifyRequest>();
            var stationLetterId = Guid.NewGuid();
            var response = await _controller.VerifyAsync(stationLetterId, "test", request);
            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.StationLetter.InvalidUrl);
        }

        [Test]
        public async Task Cannot_Verify_Not_Found()
        {
            var request = new Fixture().Create<VerifyRequest>();
            var stationLetterId = Guid.NewGuid();

            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<StationLetter, bool>>>(), It.IsAny<Func<IQueryable<StationLetter>, IQueryable<StationLetter>>>()))
                .Returns(Task.FromResult<StationLetter>(null));

            var response = await _controller.VerifyAsync(stationLetterId, "pass", request);
            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.StationLetter.NotFound);
        }

        [TestCase("pass", StationLetterStatus.Rejected)]
        [TestCase("pass", StationLetterStatus.Published)]
        [TestCase("reject", StationLetterStatus.Rejected)]
        [TestCase("reject", StationLetterStatus.Published)]
        public async Task Cannot_Verify_With_Invalid_Status(string type, StationLetterStatus status)
        {
            var request = new Fixture().Create<VerifyRequest>();
            var stationLetterId = Guid.NewGuid();
            var stationLetter = CreateStationLetterFixture().Create<StationLetter>();
            stationLetter.Status = status;

            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<StationLetter, bool>>>(), It.IsAny<Func<IQueryable<StationLetter>, IQueryable<StationLetter>>>()))
                .Returns(Task.FromResult(stationLetter));

            var response = await _controller.VerifyAsync(stationLetterId, type, request);
            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.StationLetter.InvalidStatus);
        }

        [Test]
        public async Task Can_Upload()
        {
            var players = CreatePlayerFixture()
                .CreateMany<Player>(100)
                .AsQueryable()
                .BuildMock().Object; ;
            var playerUsernames = players.Select(p => new List<string> { p.Username }).ToList();
            _uploadServiceMock.Setup(p => p.GetUploadedExcelAsync(It.IsAny<IFormFile>()))
                .Returns(Task.FromResult(playerUsernames));

            _playerQueriesMock.Setup(p => p.GetQueries())
                .Returns(players);

            var expectedPlayers = players
                .ToArray();

            var response = await _controller.UploadAsync(It.IsAny<IFormFile>());
            response.Valid.Should().BeTrue();
            for (var i = 0; i < response.Players.Length; i++)
            {
                response.Players[i].Username.Should().Be(expectedPlayers.ElementAt(i).Username);
                response.Players[i].Exists.Should().Be(expectedPlayers.ElementAt(i).Status == PlayerStatus.Enabled);
            }
        }

        [Test]
        public async Task Can_Export()
        {
            var response = await _controller.ExportAsync();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        private Fixture CreateStationLetterFixture()
        {
            var fixture = new Fixture();
            fixture.Customize<StationLetter>(u => u
                .With(s => s.Players, new StationLetterPlayer[] { })
            );
            return fixture;
        }

        private Fixture CreatePlayerFixture()
        {
            var fixture = new Fixture();
            fixture.Customize<Player>(u => u
                .With(s => s.Vip, new VipLevel { Id = Guid.NewGuid(), Name = "Vip" })
                .With(s => s.Language, new Language())
                .With(s => s.Licensee, new Licensee())
                .With(s => s.ParentPlayer, new Player())
                .With(s => s.BetLimitGroup, new BetLimitGroup())

                .With(s => s.SubPlayers, new Player[] { })
                .With(s => s.Banks, new[] { new PlayerBank
                {
                    Id = Guid.NewGuid(),
                    Username = Generator.RandomString(5),
                    BankName = Generator.RandomString(20),
                    BankAccount = Generator.RandomIntString(20),
                }})
                .With(s => s.PlayerLoginRecords, new PlayerLoginRecord[] { })
                .With(s => s.Letters, new StationLetterPlayer[] { })
                .With(s => s.Wallets, new[] { new Wallet { Balance = 0 } }));
            return fixture;
        }
    }
}
