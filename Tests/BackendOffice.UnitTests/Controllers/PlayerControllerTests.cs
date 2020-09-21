using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers;
using TGS.Apis.BackendOffice.Controllers.Messages.Players;
using TGS.Apis.BackendOffice.DataService.Repositories;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Security;
using Resources = TGS.Apis.BackendOffice.Resources;

namespace BackendOffice.UnitTests.Controllers
{
    [TestFixture, Parallelizable]
    public class PlayerControllerTests : BaseTests
    {
        private PlayerController _controller;

        private readonly Mock<ITgsQueries<Player>> _queriesMock = new Mock<ITgsQueries<Player>>();
        private readonly Mock<ITgsQueries<AuditLog>> _auditLogQueriesMock = new Mock<ITgsQueries<AuditLog>>();
        private readonly Mock<ITgsQueries<PlayerBank>> _bankQueriesMock = new Mock<ITgsQueries<PlayerBank>>();
        private readonly Mock<ITgsCommands<Player>> _commandsMock = new Mock<ITgsCommands<Player>>();
        private readonly Mock<ITgsCommands<PlayerBank>> _bankCommandsMock = new Mock<ITgsCommands<PlayerBank>>();
        private readonly Mock<ITgsCommands<AuditLog>> _auditLogCommandsMock = new Mock<ITgsCommands<AuditLog>>();
        private readonly Mock<IHashProvider> _hashProviderMock = new Mock<IHashProvider>();
        private readonly Mock<ITgsHashConfig> _tgsHashConfigMock = new Mock<ITgsHashConfig>();

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _controller = new PlayerController(
                _hashProviderMock.Object,
                _tgsHashConfigMock.Object,
                _queriesMock.Object,
                _auditLogQueriesMock.Object,
                _bankQueriesMock.Object,
                _commandsMock.Object,
                _bankCommandsMock.Object,
                _auditLogCommandsMock.Object,
                HttpContextAccessorMock.Object);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Can_Get_Players(bool showAll)
        {
            var request = new GetPlayersRequest
            {
                Page = 1,
                Rows = 10,
                SortBy = "Username",
                OrderBy = SortOrder.Descending,
                ShowAll = showAll
            };

            var list = CreatePlayerFixture()
                .CreateMany<Player>(105)
                .AsQueryable()
                .BuildMock().Object;

            var (skip, take) = GetPage(request);
            var orderedPlayers = list.OrderByDescending(p => p.Username);

            var expectedPlayers = showAll ?
                orderedPlayers.ToArray() :
                orderedPlayers.Skip(skip).Take(take).ToArray();

            _queriesMock.Setup(x => x.GetQueries())
                .Returns(list);

            var response = await _controller.GetListAsync(request);
            response.Valid.Should().BeTrue();

            for (var i = 0; i < response.Items.Length; i++)
            {
                response.Items[i].PlayerCode.Should().Be(expectedPlayers[i].PlayerCode);
                response.Items[i].Username.Should().Be(expectedPlayers[i].Username);
                response.Items[i].VipLevel.Should().Be(expectedPlayers[i].Vip.Name);
                response.Items[i].Status.Should().Be(expectedPlayers[i].Status.ToString());
                response.Items[i].HasAgent.Should().Be(expectedPlayers[i].HasAgent);
            }

            _queriesMock.Verify(p => p.GetQueries());
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Can_Get_Player_Operations(bool showAll)
        {
            var playerCode = Generator.RandomString(12);

            var request = new GetPlayerOperationRequest
            {
                Page = 1,
                Rows = 10,
                SortBy = "CreatedOn",
                OrderBy = SortOrder.Descending,
                ShowAll = showAll
            };

            var list = new Fixture()
                .CreateMany<AuditLog>(105)
                .AsQueryable()
                .BuildMock().Object;

            var (skip, take) = GetPage(request);
            var orderedPlayers = list.OrderByDescending(p => p.CreatedOn);

            var expectedPlayers = showAll ?
                orderedPlayers.ToArray() :
                orderedPlayers.Skip(skip).Take(take).ToArray();

            _auditLogQueriesMock.Setup(x => x.GetQueries())
                .Returns(list);

            var response = await _controller.GetPlayerOperationsAsync(playerCode, request);
            response.Valid.Should().BeTrue();

            for (var i = 0; i < response.Items.Length; i++)
            {
                response.Items[i].CreatedOn.Should().Be(expectedPlayers[i].CreatedOn);
                response.Items[i].CreatedByUsername.Should().Be(expectedPlayers[i].CreatedByUsername);
                response.Items[i].Operation.Should().Be(expectedPlayers[i].Operation);
            }

            _auditLogQueriesMock.Verify(p => p.GetQueries());
        }

        [Test]
        public async Task Cannot_Get_Player_Not_Found()
        {
            var playerCode = Generator.RandomString(12);
            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Player, bool>>>(), It.IsAny<Func<IQueryable<Player>, IQueryable<Player>>>()))
                .Returns(Task.FromResult<Player>(null));

            var response = await _controller.GetSingleAsync(playerCode);
            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.Player.NotFound);
        }

        [Test]
        public async Task Can_Get_Player()
        {
            var player = CreatePlayerFixture().Create<Player>();
            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Player, bool>>>(), It.IsAny<Func<IQueryable<Player>, IQueryable<Player>>>()))
                .Returns(Task.FromResult(player));

            var response = await _controller.GetSingleAsync(player.PlayerCode);
            response.Valid.Should().BeTrue();
            response.Player.Status.Should().Be(player.Status.ToString());
            response.Player.Username.Should().Be(player.Username);
            response.Player.RealName.Should().Be(player.RealName);
            response.Player.VipLevel.Should().Be(player.Vip.Name);
            response.Player.HasAgent.Should().Be(player.HasAgent);
            response.Player.RegisteredOn.Should().Be(player.RegisteredOn);
            response.Player.ParentUsername.Should().Be(player.ParentPlayer.Username);
            response.Player.Balance.Should().Be(0);
            response.Player.Phone.Should().Be(player.Phone);
            response.Player.Email.Should().Be(player.Email);
            response.Player.QQ.Should().Be(player.QQ);
            response.Player.WeChat.Should().Be(player.Wechat);

            for (var i = 0; i < response.Player.Banks.Length; i++)
            {
                response.Player.Banks[i].Id.Should().Be(player.Banks.ElementAt(i).Id);
                response.Player.Banks[i].RealName.Should().Be(player.Banks.ElementAt(i).Username);
                response.Player.Banks[i].BankName.Should().Be(player.Banks.ElementAt(i).BankName);
                response.Player.Banks[i].BankAccount.Should().Be(player.Banks.ElementAt(i).BankAccount);
            }
        }

        [Test]
        public async Task Can_Update_Player_Info()
        {
            var player = CreatePlayerFixture().Create<Player>();
            var parnetPlayer = CreatePlayerFixture().Create<Player>();

            _queriesMock.SetupSequence(s => s.GetSingleAsync(It.IsAny<Expression<Func<Player, bool>>>(), It.IsAny<Func<IQueryable<Player>, IQueryable<Player>>>()))
                .Returns(Task.FromResult(player))
                .Returns(Task.FromResult(parnetPlayer));

            var request = new Fixture().Create<UpdatePlayerRequest>();
            var response = await _controller.UpdateAsync(player.PlayerCode, "info", request);
            response.Valid.Should().BeTrue();

            _commandsMock.Verify(p => p.UpdateAsync(It.Is<Player>(a =>
                  a.RealName == request.RealName &&
                  a.VipId == request.VipId &&
                  a.ParentPlayerCode == parnetPlayer.PlayerCode &&
                  a.Status == request.Status &&
                  a.HasAgent == request.HasAgent)));

            _auditLogCommandsMock.Verify(p => p.CreateAsync(It.Is<AuditLog>(a =>
                a.PlayerCode == player.PlayerCode)));
        }

        [Test]
        public async Task Can_Update_Player_Contact()
        {
            var player = CreatePlayerFixture().Create<Player>();
            var parnetPlayer = CreatePlayerFixture().Create<Player>();

            _queriesMock.SetupSequence(s => s.GetSingleAsync(It.IsAny<Expression<Func<Player, bool>>>(), It.IsAny<Func<IQueryable<Player>, IQueryable<Player>>>()))
                .Returns(Task.FromResult(player))
                .Returns(Task.FromResult(parnetPlayer));

            var request = new Fixture().Create<UpdatePlayerRequest>();
            var response = await _controller.UpdateAsync(player.PlayerCode, "contact", request);
            response.Valid.Should().BeTrue();

            _commandsMock.Verify(p => p.UpdateAsync(It.Is<Player>(a =>
                a.Phone == request.Phone &&
                a.Email == request.Email &&
                a.QQ == request.QQ &&
                a.Wechat == request.Wechat)));

            _auditLogCommandsMock.Verify(p => p.CreateAsync(It.Is<AuditLog>(a =>
                a.PlayerCode == player.PlayerCode)));
        }

        [Test]
        public async Task Can_Update_Player_Password()
        {
            var player = CreatePlayerFixture().Create<Player>();
            var parnetPlayer = CreatePlayerFixture().Create<Player>();

            _queriesMock.SetupSequence(s => s.GetSingleAsync(It.IsAny<Expression<Func<Player, bool>>>(), It.IsAny<Func<IQueryable<Player>, IQueryable<Player>>>()))
                .Returns(Task.FromResult(player))
                .Returns(Task.FromResult(parnetPlayer));

            var request = new Fixture().Create<UpdatePlayerRequest>();

            var (salt, password) = GetCredential();

            var response = await _controller.UpdateAsync(player.PlayerCode, "password", request);

            response.Valid.Should().BeTrue();

            _commandsMock.Verify(p => p.UpdateAsync(It.Is<Player>(a =>
                a.Password == password &&
                a.Salt == salt &&
                a.UpdatedByUsername == "admin.enabled")));

            _auditLogCommandsMock.Verify(p => p.CreateAsync(It.Is<AuditLog>(a =>
                a.PlayerCode == player.PlayerCode)));
        }

        [Test]
        public async Task Cannot_Update_Player_With_Invalid_Update_Type()
        {
            var playerCode = Generator.RandomString(12);
            _queriesMock.SetupSequence(s => s.GetSingleAsync(It.IsAny<Expression<Func<Player, bool>>>(), It.IsAny<Func<IQueryable<Player>, IQueryable<Player>>>()))
                .Returns(Task.FromResult<Player>(null))
                .Returns(Task.FromResult<Player>(null));

            var request = new Fixture().Create<UpdatePlayerRequest>();

            var (salt, password) = GetCredential();

            var response = await _controller.UpdateAsync(playerCode, "info2", request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.Player.UpdateTypeNotFound);

            _commandsMock.Verify(p => p.UpdateAsync(It.Is<Player>(a =>
                a.Password == password &&
                a.Salt == salt &&
                a.UpdatedByUsername == "admin.enabled")), Times.Never);

            _auditLogCommandsMock.Verify(p => p.CreateAsync(It.Is<AuditLog>(a =>
                a.PlayerCode == playerCode)), Times.Never);
        }

        [Test]
        public async Task Cannot_Update_Player_Not_Found()
        {
            var playerCode = Generator.RandomString(12);
            _queriesMock.SetupSequence(s => s.GetSingleAsync(It.IsAny<Expression<Func<Player, bool>>>(), It.IsAny<Func<IQueryable<Player>, IQueryable<Player>>>()))
                .Returns(Task.FromResult<Player>(null))
                .Returns(Task.FromResult<Player>(null));

            var request = new Fixture().Create<UpdatePlayerRequest>();

            var (salt, password) = GetCredential();

            var response = await _controller.UpdateAsync(playerCode, "info", request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.Player.NotFound);

            _commandsMock.Verify(p => p.UpdateAsync(It.Is<Player>(a =>
                a.Password == password &&
                a.Salt == salt &&
                a.UpdatedByUsername == "admin.enabled")), Times.Never);

            _auditLogCommandsMock.Verify(p => p.CreateAsync(It.Is<AuditLog>(a =>
                a.PlayerCode == playerCode)), Times.Never);
        }

        [Test]
        public async Task Cannot_Update_Player_With_Parent_Player_Not_Found()
        {
            var player = CreatePlayerFixture().Create<Player>();
            var playerCode = Generator.RandomString(12);
            _queriesMock.SetupSequence(s => s.GetSingleAsync(It.IsAny<Expression<Func<Player, bool>>>(), It.IsAny<Func<IQueryable<Player>, IQueryable<Player>>>()))
                .Returns(Task.FromResult(player))
                .Returns(Task.FromResult<Player>(null));

            var request = new Fixture().Create<UpdatePlayerRequest>();

            var (salt, password) = GetCredential();

            var response = await _controller.UpdateAsync(playerCode, "info", request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.Player.ParentPlayerNotFound);

            _commandsMock.Verify(p => p.UpdateAsync(It.Is<Player>(a =>
                a.Password == password &&
                a.Salt == salt &&
                a.UpdatedByUsername == "admin.enabled")), Times.Never);

            _auditLogCommandsMock.Verify(p => p.CreateAsync(It.Is<AuditLog>(a =>
                a.PlayerCode == playerCode)), Times.Never);
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

        private (byte[], byte[]) GetCredential()
        {
            var salt = Generator.RandomByteArray(32);
            var hashedPassword = Generator.RandomByteArray(128);

            _hashProviderMock.Setup(p => p.GenerateRandomSalt(It.IsAny<ushort>()))
                .Returns(salt);

            _hashProviderMock.Setup(p => p.Hash(
                It.IsAny<byte[]>(),
                It.IsAny<byte[]>(),
                It.IsAny<HashAlgorithmKind>()))
                .Returns(hashedPassword);

            return (salt, hashedPassword);
        }
    }
}
