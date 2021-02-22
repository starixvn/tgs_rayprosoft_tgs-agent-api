using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Redis;
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

        protected async Task PlayerLoginAsync(IServiceProvider serviceProvider, string username,
            PlatformType platformType, string brandCode, string currencyCode)
        {
            var loginTimeCacheKey = $"{SR.LoginTime}:{username}:{platformType}";

            var loginTime = Get<DateTimeOffset?>(loginTimeCacheKey);
            if (loginTime.HasValue &&
                loginTime.Value.AddSeconds(Consts.TokenExpireSeconds.PlayerLogin) >= DateTimeOffset.UtcNow)
            {
                return;
            }

            var brandCacheKey = $"{SR.Brand}:{brandCode}";
            var brand = Get<Brand>(brandCacheKey);
            var db = GetDb<ITgsRepository>(serviceProvider);

            if (brand == null)
            {
                brand = await db.Brands.FirstAsync(b => b.Code == brandCode);
                Set(brandCacheKey, brand);
            }

            var player = await db.Players.FirstOrDefaultAsync(p => p.BrandId == brand.Id &&
                p.Username.ToLower() == username.ToLower());

            var apiClient = serviceProvider.GetRequiredService<IPlayerApiClient>();

            var brandTokenCacheKey = $"{SR.BrandToken}:{brandCode}";
            var brandToken = Get<string>(brandTokenCacheKey);

            var brandTokenExpireTimeCacheKey = $"{SR.BrandTokenExpireTime}:{brandCode}";
            var brandTokenExpireTime = Get<long>(brandTokenExpireTimeCacheKey);

            if (brandToken.IsNullOrEmpty() || brandTokenExpireTime < DateTimeOffset.Now.ToUnixTimeMilliseconds())
            {
                var r = await apiClient.GetBrandTokenAsync(new()
                {
                    ApiKey = brand.ApiKey,
                    Currency = player?.DefaultWalletCurrencyCode ?? currencyCode
                });

                brandToken = r.Token;

                Set(brandTokenCacheKey, brandToken);
                Set(brandTokenExpireTimeCacheKey, r.ExpiredOn);
            }

            const string password = "TgsP@ssw0rd";
            var headers = new Dictionary<string, string>
            {
                {"Authorization", $"Bearer {brandToken}"}
            };

            var playerCode = player?.PlayerCode;

            if (player == null)
            {
                var createResponse = await apiClient.CreatePlayerAsync(new()
                {
                    Username = username,
                    Password = password,
                    ConfirmPassword = password,
                    CurrencyCode = currencyCode,
                    IsTestPlayer = !username.StartsWith("RP", StringComparison.OrdinalIgnoreCase)
                }, headers);

                playerCode = createResponse.PlayerCode;
            }

            var res = await apiClient.PlayerLoginAsync(new()
            {
                Username = username,
                Password = password,
                PlatformType = platformType
            }, headers);

            Set(SR.BrandCode, brandCode);
            Set(SR.PlayerCode, playerCode);
            Set(SR.Username, username);
            Set(SR.PlatformType, platformType);
            Set(loginTimeCacheKey, res.LoginTime);
        }

        protected async Task SetPlayerBalanceAsync(ServiceProvider serviceProvider, decimal balance)
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var db = GetDb<ITgsRepository>(serviceProvider);

            const string sql = "update dbo.wallets w set balance=:balance where w.player_code=:player_code";
            await db.ExecuteSqlCommandAsync(sql, CommandType.Text, new Dictionary<string, object>
            {
                {":balance", balance},
                {":player_code", playerCode}
            });
        }

        protected async Task<Wallet> GetPlayerWalletAsync(ServiceProvider serviceProvider)
        {
            await using var db = serviceProvider.GetRequiredService<Func<ITgsRepository>>()();
            var playerCode = Get<string>(SR.PlayerCode);
            return await db.Wallets.FirstAsync(w => w.PlayerCode == playerCode);
        }

        protected static async Task<T> GetSettingAsync<T>(ServiceProvider serviceProvider, string key)
        {
            var db = GetDb<ITgsRepository>(serviceProvider);
            var setting = await db.Settings.FirstAsync(s => s.Key == key);

            var json = serviceProvider.GetRequiredService<IJsonSerializationProvider>();
            return json.DeserializeFromString<T>(setting.Value);
        }

        protected static async Task DeauthorizePlayerAsync(IServiceProvider serviceProvider, string playerCode)
        {
            var db = GetDb<ITgsRepository>(serviceProvider);

            var records = await db.PlayerLoginRecords
                .Where(r => r.PlayerCode == playerCode).ToArrayAsync();

            foreach (var r in records)
            {
                r.Status = LoginStatus.Deauthorized;
            }

            await db.SaveChangesAsync();

            var cacheManager = serviceProvider.GetRequiredService<IRedisCacheManager>();
            await cacheManager.ClearCacheAsync(RedisCacheType.PlayerLoginRecords, playerCode);
        }

        protected async Task VerifyRoundStatus(IServiceProvider serviceProvider, RoundStatus expectedStatus)
        {
            var externalRoundId = Get<string>(SR.ExternalRoundId);
            var db = GetDb<ITgsRepository>(serviceProvider);
            var status = await db.Rounds.Where(r => r.ExternalRoundId == externalRoundId)
                .Select(r => r.Status)
                .FirstAsync();

            status.Should().Be(expectedStatus);
        }

        protected async Task ChangeGameProviderStatusAsync(IServiceProvider serviceProvider,
            string gameProviderCode, Status status)
        {
            if (!HasTag("ChangeGameProviderStatus"))
            {
                return;
            }

            var db = GetDb<ITgsRepository>(serviceProvider);

            var gameProvider = await db.GameProviders.FirstAsync(gp => gp.Code == gameProviderCode);
            gameProvider.Status = status;

            await db.SaveChangesAsync();

            var cacheManager = serviceProvider.GetRequiredService<IRedisCacheManager>();
            await cacheManager.ClearCacheAsync(RedisCacheType.GameProvider, gameProvider.Id);
        }

        protected async Task ChangeGameProviderConfigurationStatusAsync(IServiceProvider serviceProvider,
            string gameProviderCode, Status status)
        {
            if (!HasTag("ChangeGameProviderConfigurationStatus"))
            {
                return;
            }

            var brandCode = Get<string>(SR.BrandCode);

            var db = GetDb<ITgsRepository>(serviceProvider);

            var gameProviderConfiguration = await (
                from bgp in db.BrandGameProviders
                join brand in from b in db.Brands where b.Code == brandCode select b
                    on bgp.BrandId equals brand.Id
                join gameProvider in from gp in db.GameProviders where gp.Code == gameProviderCode select gp
                    on bgp.GameProviderId equals gameProvider.Id
                join gpc in db.GameProviderConfigurations
                    on bgp.GameProviderConfigurationId equals gpc.Id
                select gpc).FirstAsync();

            gameProviderConfiguration.Status = status;

            await db.SaveChangesAsync();

            var cacheManager = serviceProvider.GetRequiredService<IRedisCacheManager>();
            await cacheManager.ClearCacheAsync(RedisCacheType.GameProviderConfiguration,
                gameProviderConfiguration.GameProviderId);
        }

        protected async Task ChangeGameStatusAsync(IServiceProvider serviceProvider, string gameProviderCode,
            string externalGameId, GameStatus gameStatus)
        {
            if (!HasTag("ChangeGameStatus"))
            {
                return;
            }

            var platformType = Get<PlatformType>(SR.PlatformType);

            var db = GetDb<ITgsRepository>(serviceProvider);

            var game = await (
                from g in db.Games
                join gameProvider in from gp in db.GameProviders where gp.Code == gameProviderCode select gp
                    on g.GameProviderId equals gameProvider.Id
                where g.ExternalId == externalGameId && g.PlatformType == platformType
                select g).FirstAsync();

            game.Status = gameStatus;

            await db.SaveChangesAsync();

            var cacheManager = serviceProvider.GetRequiredService<IRedisCacheManager>();
            await cacheManager.ClearCacheAsync(RedisCacheType.Games, game.GameProviderId);
        }

        protected async Task ChangeBrandStatusAsync(IServiceProvider serviceProvider, Status status)
        {
            if (!HasTag("ChangeBrandStatus"))
            {
                return;
            }

            var brandCode = Get<string>(SR.BrandCode);

            var db = GetDb<ITgsRepository>(serviceProvider);

            var brand = await db.Brands.FirstAsync(b => b.Code == brandCode);
            brand.Status = status;

            await db.SaveChangesAsync();

            var cacheManager = serviceProvider.GetRequiredService<IRedisCacheManager>();
            await cacheManager.ClearCacheAsync(RedisCacheType.Brand, brand.Id);
        }

        #region Private Methods

        #endregion
    }
}