using NUnit.Framework;
using TGS.Apis.TgsApi.GameProviders.PragmaticPlay;
using TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Controllers;
using TGS.Core.Models;

namespace TGS.UnitTests.Startups.TgsApi
{
    [TestFixture]
    public sealed class PragmaticPlayStartupTests : TgsApiStartupTestsBase<PragmaticPlayStartup>
    {
        public PragmaticPlayStartupTests()
            : base(Consts.GameProvider.PragmaticPlay)
        {
        }

        [Test]
        public void Can_Get_All_Registered_Types() =>
            ResolveAllRegistrationsWithAdditionalTypes(new[] {typeof(PragmaticPlayGameController)});
    }
}