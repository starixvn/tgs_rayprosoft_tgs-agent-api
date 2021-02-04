using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;
using TGS.Apis.OperatorApi.Models.Messages.Players;
using TGS.Core.DataServices.Bet.Extensions;
using TGS.Core.DataServices.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Tests.ApiClients;
using TGS.Tests.TestsCommon;

namespace TGS.Tests.Games.IntegrationTests.GameProviders.Steps
{
    [Binding, Scope(Tag = "SBOBET")]
    public sealed class SBOBETEndpoint : BaseGameEndpoint
    {
        public SBOBETEndpoint(ScenarioContext context)
            : base(context)
        {
        }

        protected override void AddRequiredServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTgsRepository(configuration, "Tgs");
            services.AddBetRepository(configuration, "Bet");

            services.AddJson()
                    .AddApiClient();

            var apiUrlSection = configuration.GetSection("ApiUrls");
            services.AddHttpClient<IOperatorApiClient, OperatorApiClient>(c =>
                c.BaseAddress = new Uri(apiUrlSection.GetValue<string>("Operator")));
        }

        [Given(@"player ""(.*)"" login via ""(.*)""")]
        public async Task GivenPlayerLoginViaAsync(string username, PlatformType platformType)
        {
            var key = $"{SR.LoginTime}:{username}:{platformType}";

            var loginTime = Get<DateTimeOffset?>(key);
            if (loginTime.HasValue &&
                loginTime.Value.AddSeconds(Consts.Player.LoginExpirationSeconds) >= DateTimeOffset.UtcNow)
            {
                return;
            }

            var apiClient = ServiceProvider.GetRequiredService<IOperatorApiClient>();

            var res = await apiClient.PlayerLoginAsync(new LoginRequest
            {
                Username = username,
                Password = "TgsP@ssw0rd",
                PlatformType = platformType
            });

            Set(SR.PlayerCode, res.PlayerCode);
            Set(key, res.LoginTime);
        }
    }
}