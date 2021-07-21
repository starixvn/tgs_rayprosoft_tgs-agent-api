using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using StackExchange.Redis;
using TGS.Core.Infrastructures.Redis;
using TGS.Promotion.Repositories;

namespace Promotion.Repositories.UnitTests
{
    [TestFixture]
    public class RedisPlayerDataRepositoryTests
    {
        [Test]
        public async Task CheckPlayerFirstDepositing_If_Key_Exists_Should_Return_True()
        {
            var fakeRedisRepository = Substitute.For<IRedisRepository>();
            var fakeRedisDatabase = Substitute.For<IDatabase>();

            fakeRedisRepository.GetDb()
                .Returns(fakeRedisDatabase);

            fakeRedisDatabase.KeyExistsAsync(Arg.Any<RedisKey>(), Arg.Any<CommandFlags>())
                .Returns(true);

            var sut = new RedisPlayerDataRepository(fakeRedisRepository);

            var actual = await sut.CheckPlayerFirstDepositingAsync("ThisIsPlayerCode");

            Assert.IsTrue(actual);
        }
    }
}
