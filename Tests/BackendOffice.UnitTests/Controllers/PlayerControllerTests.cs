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


        [Test]
        public async Task Can_Get_Player()
        {
            var Player = CreatePlayerFixture().Create<Player>();
            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<Player, bool>>>(), It.IsAny<Func<IQueryable<Player>, IQueryable<Player>>>()))
                .Returns(Task.FromResult(Player));

            var response = await _controller.GetSingleAsync(Player.PlayerCode);
            response.Valid.Should().BeTrue();
            //response.Player.Id.Should().Be(Player.Id);
            //response.Player.Name.Should().Be(Player.Name);
            //response.Player.DisplayName.Should().Be(Player.DisplayName);
            //response.Player.Description.Should().Be(Player.Description);
            //response.Player.CreatedByUsername.Should().Be(Player.CreatedByUsername);
            //response.Player.UpdatedByUsername.Should().Be(Player.UpdatedByUsername);
            //response.Player.CreatedOn.Should().Be(Player.CreatedOn);
            //response.Player.UpdatedOn.Should().Be(Player.UpdatedOn);

            //for (var i = 0; i < response.Player.Menus.Length; i++)
            //{
            //    response.Player.Menus[i].Code.Should().Be(menuSettings[i].Code);
            //    response.Player.Menus[i].IsDisplay.Should().Be(menuSettings[i].IsDisplay);

            //    for (var j = 0; j < response.Player.Banks.Length; j++)
            //    {
            //        var actualMenu = response.Player.Banks[j];
            //        var expectedMenu = menuSettings[i].SubMenus[j];
            //        actualMenu.Code.Should().Be(expectedMenu.Code);
            //        actualMenu.IsDisplay.Should().Be(expectedMenu.IsDisplay);
            //    }
            //}
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
                .With(s => s.Wallets, new Wallet[] { }));
            return fixture;
        }
    }
}
