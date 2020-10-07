using NUnit.Framework;
using TGS.Apis.TgsApi.GameProviders.RedTiger;
using TGS.Apis.TgsApi.GameProviders.RedTiger.Controllers;

namespace Tgs.UnitTests.TgsApi.Startups
{
    [TestFixture]
    public class RedTigerStartupTests : StartupTestsBase
    {
        [Test]
        public void Can_Get_All_Registered_Types()
        {
            ResolveRegistrationsAndControllers<RedTigerStartup>(new[] {typeof(RedTigerGameController) });
        }
    }
}
