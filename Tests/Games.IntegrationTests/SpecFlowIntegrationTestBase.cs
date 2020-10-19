using System;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Tests.TestsCommon;

namespace TGS.Tests.Games.IntegrationTests
{
    public abstract class SpecFlowIntegrationTestBase : SpecFlowBase
    {
        protected SpecFlowIntegrationTestBase(ScenarioContext context)
            : base(context)
        {
            
        }

        protected static T GetDb<T>(IServiceProvider sp) where T : IBaseRepository =>
            sp.GetRequiredService<T>();
    }
}
