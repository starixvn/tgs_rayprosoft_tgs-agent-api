using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers;
using TGS.Apis.BackendOffice.DataService.Repositories;
using TGS.Core.DataServices.Models;

namespace BackendOffice.UnitTests.Controllers
{
    [TestFixture, Parallelizable]
    public class StationLetterControllerTests : BaseTests
    {
        private StationLetterController _controller;

        private readonly Mock<ITgsQueries<Player>> _playerQueriesMock = new Mock<ITgsQueries<Player>>();
        private readonly Mock<ITgsQueries<StationLetter>> _stationLetterQueriesMock = new Mock<ITgsQueries<StationLetter>>();
        private readonly Mock<ITgsCommands<StationLetter>> _commandsMock = new Mock<ITgsCommands<StationLetter>>();
        private readonly Mock<ITgsCommands<StationLetterPlayer>> _bankCommandsMock = new Mock<ITgsCommands<StationLetterPlayer>>();

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _controller = new StationLetterController(
                _stationLetterQueriesMock.Object,
                _playerQueriesMock.Object,
                _commandsMock.Object,
                _bankCommandsMock.Object,
                HttpContextAccessorMock.Object);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Can_Get_Players(bool showAll)
        {
            //var request = new GetPlayersRequest
            //{
            //    Page = 1,
            //    Rows = 10,
            //    SortBy = "Username",
            //    OrderBy = SortOrder.Descending,
            //    ShowAll = showAll
            //};

            //var list = CreatePlayerFixture()
            //    .CreateMany<Player>(105)
            //    .AsQueryable()
            //    .BuildMock().Object;

            //var (skip, take) = GetPage(request);
            //var orderedPlayers = list.OrderByDescending(p => p.Username);

            //var expectedPlayers = showAll ?
            //    orderedPlayers.ToArray() :
            //    orderedPlayers.Skip(skip).Take(take).ToArray();

            //_queriesMock.Setup(x => x.GetQueries())
            //    .Returns(list);

            //var response = await _controller.GetListAsync(request);
            //response.Valid.Should().BeTrue();

            //for (var i = 0; i < response.Items.Length; i++)
            //{
            //    response.Items[i].PlayerCode.Should().Be(expectedPlayers[i].PlayerCode);
            //    response.Items[i].Username.Should().Be(expectedPlayers[i].Username);
            //    response.Items[i].VipLevel.Should().Be(expectedPlayers[i].Vip.Name);
            //    response.Items[i].Status.Should().Be(expectedPlayers[i].Status.ToString());
            //    response.Items[i].HasAgent.Should().Be(expectedPlayers[i].HasAgent);
            //}

            //_queriesMock.Verify(p => p.GetQueries());
        }
    }
}
