// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using NUnit.Framework;
using TGS.Apis.TgsApi.GameProviders.SBOBET;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Controllers;
using TGS.Core.Models;

namespace TGS.UnitTests.Startups.TgsApi
{
    [TestFixture]
    public sealed class SBOBETStartupTests : TgsApiStartupTestsBase<SBOBETStartup>
    {
        public SBOBETStartupTests()
            : base(Consts.GameProvider.SBOBET)
        {
        }

        [Test]
        public void Can_Get_All_Registered_Types()
        {
            ResolveAllRegistrationsWithAdditionalTypes(new[] {typeof(SBOBETGameController)});
        }
    }
}