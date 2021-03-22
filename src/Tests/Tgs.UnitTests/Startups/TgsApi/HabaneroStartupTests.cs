// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using NUnit.Framework;
using TGS.Apis.TgsApi.GameProviders.Habanero;
using TGS.Apis.TgsApi.GameProviders.Habanero.Controllers;
using TGS.Core.Models;

namespace TGS.UnitTests.Startups.TgsApi
{
    [TestFixture]
    public sealed class HabaneroStartupTests : TgsApiStartupTestsBase<HabaneroStartup>
    {
        public HabaneroStartupTests()
            : base(Consts.GameProvider.Habanero)
        {
        }

        [Test]
        public void Can_Get_All_Registered_Types() =>
            ResolveAllRegistrationsWithAdditionalTypes(new[] {typeof(HabaneroGameController)});
    }
}