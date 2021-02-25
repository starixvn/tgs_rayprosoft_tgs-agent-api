using System.Threading.Tasks;
using CliWrap;
using NUnit.Framework;

namespace TGS.Tests.Redis.IntegrationTests
{
    [SetUpFixture]
    public class TestsSetup
    {
        [OneTimeSetUp]
        public Task TestFixtureSetUpAsync()
        {
            return Cli.Wrap("docker")
                .WithArguments("run -d --rm --name redistest -p 6379:6379 redis")
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync();
        }

        [OneTimeTearDown]
        public Task FinalTearDownAsync()
        {
            return Cli.Wrap("docker")
                .WithArguments("stop redistest")
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync();
        }
    }
}
