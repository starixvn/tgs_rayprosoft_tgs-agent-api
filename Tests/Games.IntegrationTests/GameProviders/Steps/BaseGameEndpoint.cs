using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;
using TGS.Apis.OperatorApi.Models.Messages.Players;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Tests.ApiClients.Operators;
using TGS.Tests.TestsCommon;

namespace TGS.Tests.Games.IntegrationTests.GameProviders.Steps
{
    public abstract class BaseGameEndpoint : SpecFlowIntegrationTestBase
    {
        protected BaseGameEndpoint(ScenarioContext context)
            : base(context)
        {
        }

        protected async Task PlayerLoginAsync(string username, PlatformType platformType, string licenseeCode,
            IServiceProvider serviceProvider)
        {
            var loginTimeCacheKey = $"{SR.LoginTime}:{username}:{platformType}";

            var loginTime = Get<DateTimeOffset?>(loginTimeCacheKey);
            if (loginTime.HasValue &&
                loginTime.Value.AddSeconds(Consts.Player.LoginExpirationSeconds) >= DateTimeOffset.UtcNow)
                return;

            var licenseeCacheKey = $"{SR.Licensee}:{licenseeCode}";
            var licensee = Get<Licensee>(licenseeCacheKey);

            if (licensee == null)
            {
                var db = GetDb<ITgsRepository>(serviceProvider);
                licensee = await db.Licensees.FirstAsync(l => l.Code == licenseeCode);
                Set(licenseeCacheKey, licensee);
            }

            var timestamp = DateTimeOffset.Now.ToIso8601String();
            var hashProvider = serviceProvider.GetRequiredService<IHashProvider>();
            var authCode = hashProvider.Hash(
                $"{licensee.ClientId}{licensee.SecretKey}{timestamp}",
                HashAlgorithmKind.Sha512,
                HashStringKind.Hex,
                Encoding.UTF8);

            var headers = new Dictionary<string, string>
            {
                {"clientid", licensee.ClientId},
                {"timestamp", timestamp},
                {"authcode", authCode}
            };

            var apiClient = serviceProvider.GetRequiredService<IPlayerApiClient>();

            var res = await apiClient.PlayerLoginAsync(new LoginRequest
            {
                Username = username,
                Password = "TgsP@ssw0rd",
                PlatformType = platformType
            }, headers);

            Set(SR.PlayerCode, res.PlayerCode);
            Set(loginTimeCacheKey, res.LoginTime);
        }

        protected async Task SetPlayerBalanceAsync(ServiceProvider serviceProvider, decimal balance)
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var db = GetDb<ITgsRepository>(serviceProvider);

            var sql = "UPDATE Wallets SET Balance=@balance WHERE PlayerCode=@playerCode";
            await db.ExecuteSqlCommandAsync(sql, CommandType.Text, parameters: new SqlParameter[]
            {
                new()
                {
                   ParameterName = "@balance",
                   Value = balance,
                   DbType = DbType.Decimal
                },
                new()
                {
                    ParameterName = "@playerCode",
                    Value = playerCode,
                    DbType = DbType.String
                },
            });
        }

        protected Task<Wallet> GetPlayerWalletAsync(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetRequiredService<Func<ITgsRepository>>()();
            var playerCode = Get<string>(SR.PlayerCode);
            return db.Wallets.FirstAsync(w => w.PlayerCode == playerCode);
        }

        protected static async Task<T> GetSettingAsync<T>(ServiceProvider serviceProvider, string key)
        {
            var db = GetDb<ITgsRepository>(serviceProvider);
            var setting = await db.Settings.FirstAsync(s => s.Key == key);

            var json = serviceProvider.GetRequiredService<IJsonSerializationProvider>();
            return json.DeserializeFromString<T>(setting.Value);
        }

        #region Private Methods

        #endregion
    }
}