using NUnit.Framework;
using TGS.Apis.TgsApi.GameProviders.RedTiger;
using TGS.Apis.TgsApi.GameProviders.RedTiger.Controllers;
using TGS.Core.Models;

namespace TGS.UnitTests.Startups.TgsApi
{
    [TestFixture]
    public class RedTigerStartupTestsTests : TgsApiStartupTestsBase<RedTigerStartup>
    {
        public RedTigerStartupTestsTests()
            : base(Consts.GameProvider.RedTiger)
        {
        }

        [Test]
        [Ignore("not implementation")]
        public void Can_Get_All_Registered_Types()
        {
            ResolveAllRegistrationsWithControllers(new[] {typeof(RedTigerGameController)});
        }
    }
}