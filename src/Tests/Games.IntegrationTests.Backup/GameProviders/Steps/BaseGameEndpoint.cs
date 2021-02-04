using System;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Tests.TestsCommon;

namespace TGS.Tests.Games.IntegrationTests.GameProviders.Steps
{
    public abstract class BaseGameEndpoint : SpecFlowIntegrationTestBase
    {
        protected BaseGameEndpoint(ScenarioContext context)
            : base(context)
        {
        }

    }
}