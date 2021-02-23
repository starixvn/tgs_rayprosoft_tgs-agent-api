using System.Threading.Tasks;
using CliWrap;
using NUnit.Framework;

namespace TGS.Tests.Nats.IntegrationTests
{
    [SetUpFixture]
    public class TestsSetup
    {
        [OneTimeSetUp]
        public Task TestFixtureSetUpAsync()
        {
            return Cli.Wrap("docker")
                .WithArguments("run -d --rm --name natstest -p 4222:4222 nats")
                .ExecuteAsync();
        }

        [OneTimeTearDown]
        public Task FinalTearDownAsync()
        {
            return Cli.Wrap("docker")
                .WithArguments("stop natstest")
                .ExecuteAsync();
        }
    }
}
