using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Utility;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.DataInitializers
{
    [ExcludeFromCodeCoverage]
    public sealed partial class DataInitializer : IDataInitializer
    {
        private static readonly DateTimeOffset Now = DateTimeOffset.UtcNow;
        private static readonly object Locker = new();
        private static readonly string AdminUsername = "admin.enabled";

        private static readonly IList<IReseller> Resellers = new IReseller[]
        {
            new TgsReseller()
        };

        private readonly IDictionary<string, Brand> _brandCreated =
            new Dictionary<string, Brand>();

        private readonly CurrencyDefined[] _currencies =
        {
            new("CNY", "Yuan", "元", "¥"),
            new("EUR", "Euro", "Euro", "€"),
            new("GBP", "Pound Sterling", "Pound Sterling", "£"),
            new("IDR", "Indonesian rupiah", "rupiah Indonesia", "Rp"),
            new("IDR_1000", "Indonesian rupiah", "rupiah Indonesia", "Rp"),
            new("INR", "Indian Rupee", "भारतीय रुपये", "₹"),
            new("JPY", "Japanese Yen", "円", "¥"),
            new("KRW", "Korean won", "Korean won", "₩"),
            new("MYR", "Malaysian Ringgit", "Malaysian Ringgit", "RM"),
            new("SGD", "Singapore Dollar", "Singapore Dollar", "$"),
            new("THB", "Thai Baht", "บาท", "฿"),
            new("USD", "United States Dollar", "United States Dollar", "$"),
            new("VND", "Vietnamese Dong", "đồng", "₫"),
            new("VND_1000", "Vietnamese Dong", "đồng", "₫")
        };

        private readonly IList<IGameProvider> _gameProviders;

        private readonly IDictionary<string, GameProvider> _gameProvidersCreated =
            new Dictionary<string, GameProvider>();

        private readonly IJsonSerializationProvider _jsonSerializationProvider;

        private readonly StringBuilder _logBuilder = new();
        private readonly IPasswordHandler _passwordHandler;
        private readonly ITgsRepository _repository;

        private readonly IRole[] _roles =
        {
            new AdminRole(), new ResellerAdminRole(), new LicenseeAdminRole(), new BrandRole()
        };

        private readonly VipLevel[] _vipLevels =
        {
            new() {Id = GuidExtensions.NewSequentialGuid(), Code = "VIP 0", Name = "名称A"},
            new() {Id = GuidExtensions.NewSequentialGuid(), Code = "VIP 1", Name = "名称B"},
            new() {Id = GuidExtensions.NewSequentialGuid(), Code = "VIP 2", Name = "名称C"},
            new() {Id = GuidExtensions.NewSequentialGuid(), Code = "VIP 3", Name = "名称D"},
            new() {Id = GuidExtensions.NewSequentialGuid(), Code = "VIP 4", Name = "名称E"},
            new() {Id = GuidExtensions.NewSequentialGuid(), Code = "VIP 5", Name = "名称F"}
        };

        private BetLimit[] _betLimits;

        private Player[] _players;

        public DataInitializer(ITgsRepository repository, IWebHostEnvironment env)
        {
            _repository = repository;

            var hashProvider = (IHashProvider) new HashProvider();
            _passwordHandler = new PasswordHandler(hashProvider);
            _jsonSerializationProvider = new JsonSerializationProvider();

            _gameProviders = new IGameProvider[]
            {
                new SBOGameProvider(env)
            };
        }

        async Task IDataInitializer.SetInitialDataAsync()
        {
            if (_repository.Players.Any())
            {
                return;
            }

            var timer = new Stopwatch();
            timer.Start();

            await LogSeedingTimeAsync(GenerateCurrenciesAsync, _logBuilder);
            await LogSeedingTimeAsync(GenerateLanguagesAsync, _logBuilder);
            await LogSeedingTimeAsync(GenerateBetLimitsAsync, _logBuilder);
            await LogSeedingTimeAsync(GenerateVipLevelAsync, _logBuilder);
            await LogSeedingTimeAsync(GenerateGameProvidersAsync, _logBuilder);
            await LogSeedingTimeAsync(GenerateResellersWithDescendantsAsync, _logBuilder);
            await LogSeedingTimeAsync(GeneratePlayersAsync, _logBuilder);
            await LogSeedingTimeAsync(GenerateSettingsAsync, _logBuilder);
            await LogSeedingTimeAsync(GenerateLocalizedStringsAsync, _logBuilder);
            await LogSeedingTimeAsync(GeneratePaymentAsync, _logBuilder);
            await LogSeedingTimeAsync(GenerateDepartmentsAsync, _logBuilder);
            await LogSeedingTimeAsync(GenerateRolesAsync, _logBuilder);
            await LogSeedingTimeAsync(GenerateUsersAsync, _logBuilder);
            await LogSeedingTimeAsync(GenerateSportTypesAndMarketTypesAsync, _logBuilder);
            await LogSeedingTimeAsync(GeneratePlayerSportsBetLimitsAsync, _logBuilder);

            timer.Stop();

            _logBuilder.AppendLine($"Total Time was {timer.ElapsedMilliseconds} ms");
            Console.WriteLine(_logBuilder.ToString());
        }

        private Task GenerateCurrenciesAsync()
        {
            var currencies = _currencies
                .Select(c => new Currency
                {
                    Code = c.Code,
                    Name = c.Name,
                    NativeName = c.NativeName,
                    Symbol = c.Symbol,
                    CreatedByUsername = AdminUsername,
                    UpdatedByUsername = AdminUsername,
                    CreatedOn = Now,
                    UpdatedOn = Now
                }).ToArray();

            return _repository.BulkInsertAsync(currencies);
        }

        private Task GenerateVipLevelAsync()
        {
            foreach (var vip in _vipLevels)
            {
                vip.CreatedByUsername = AdminUsername;
                vip.UpdatedByUsername = AdminUsername;
                vip.CreatedOn = Now;
                vip.UpdatedOn = Now;
            }

            return _repository.BulkInsertOrUpdateAsync(_vipLevels);
        }

        private Task GenerateLanguagesAsync()
        {
            var languages = new[]
                {
                    new {CultureCode = "en-US", LanguageCode = "en", Name = "English", LocalizedName = "English"},
                    new
                    {
                        CultureCode = "id-ID", LanguageCode = "id", Name = "Indonesian",
                        LocalizedName = "bahasa Indonesia"
                    },
                    new {CultureCode = "ja-JP", LanguageCode = "ja", Name = "Japanese", LocalizedName = "日本語"},
                    new {CultureCode = "ko-KR", LanguageCode = "ko", Name = "Korean", LocalizedName = "한국어 / 조선말"},
                    new {CultureCode = "th-TH", LanguageCode = "th", Name = "Thai", LocalizedName = "ภาษาไทย"},
                    new
                    {
                        CultureCode = "vi-VN", LanguageCode = "vi", Name = "Vietnamese",
                        LocalizedName = "ngôn ngữ tiếng Việt"
                    },
                    new
                    {
                        CultureCode = "zh-CN", LanguageCode = "cn", Name = "Simplified Chinese", LocalizedName = "简体中文"
                    },
                    new
                    {
                        CultureCode = "zh-TW", LanguageCode = "zh", Name = "Traditional Chinese",
                        LocalizedName = " 繁體中文"
                    }
                }
                .Select(p => new Language
                {
                    CultureCode = p.CultureCode,
                    LanguageCode = p.LanguageCode,
                    Name = p.Name,
                    LocalizedName = p.LocalizedName,
                    CreatedByUsername = AdminUsername,
                    UpdatedByUsername = AdminUsername,
                    CreatedOn = Now,
                    UpdatedOn = Now
                }).ToArray();

            return _repository.BulkInsertAsync(languages);
        }

        private async Task GenerateBetLimitsAsync()
        {
            BetLimitDefined[] limits =
            {
                new("CNY", 1, 200, 1000),
                new("CNY", 10, 300, 1000),
                new("EUR", 1, 200, 1000),
                new("GBP", 1, 200, 1000),
                new("IDR", 1000, 200000, 1000000),
                new("IDR_1000", 1, 200, 1000),
                new("INR", 1, 200, 1000),
                new("JPY", 1, 200, 1000),
                new("KRW", 1, 200, 1000),
                new("MYR", 1, 200, 1000),
                new("SGD", 1, 200, 1000),
                new("THB", 1, 200, 1000),
                new("USD", 1, 200, 1000),
                new("VND", 1000, 200000, 1000000),
                new("VND_1000", 1, 200, 1000)
            };

            var betLimits = limits.Select(b => new BetLimit
            {
                CurrencyCode = b.CurrencyCode,
                MinBet = b.MinBet,
                MaxBet = b.MaxBet,
                MaxBetPerMatch = b.MaxBetPerMatch,
                CreatedOn = Now,
                UpdatedOn = Now,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername
            }).ToArray();

            await _repository.BulkInsertAsync(betLimits);

            _betLimits = await _repository.BetLimits.ToArrayAsync();
        }

        private async Task GeneratePlayersAsync()
        {
            var rnd = new Random();
            var (salt, password) = _passwordHandler.GenerateCredential("TgsP@ssw0rd");
            var playerCodes = new List<string>();
            var invitationCodes = new List<string>();

            var tgsPlayer = GenerateTgsPlayers();

            _players = tgsPlayer.Select(p =>
            {
                var playerCode = PlayerCodeGenerator.NewId();
                var vipId = _vipLevels[rnd.Next(0, 6)].Id;
                var invitationCode = Generator.RandomString(6);

                return CreatePlayer(playerCode, p.Username, p.RealName, salt, password,
                    vipId, p.Type, invitationCode, brandId: p.BrandId, currencyCode: p.CurrencyCode);
            }).ToArray();

            var randomPlayers = Enumerable.Range(1, 20)
                .Select(p =>
                {
                    var playerCode = PlayerCodeGenerator.NewId();
                    var parentPlayerCode = playerCodes.Count == 0 ? null :
                        rnd.NextDouble() > 0.5 ? null : playerCodes[rnd.Next(0, playerCodes.Count - 1)];

                    var invitationCode = Generator.RandomString(6);
                    var parentInvitationCode = invitationCodes.Count == 0 ? null :
                        rnd.NextDouble() > 0.5 ? null : invitationCodes[rnd.Next(0, invitationCodes.Count - 1)];

                    playerCodes.Add(playerCode);
                    invitationCodes.Add(invitationCode);

                    var username = Generator.RandomString(12);
                    var realName = Generator.RandomString(12);
                    var vipId = _vipLevels[rnd.Next(0, 6)].Id;

                    return CreatePlayer(playerCode, username, realName, salt, password,
                        vipId, PlayerType.RealPlayer, invitationCode, parentPlayerCode, parentInvitationCode);
                }).ToArray();

            _players = _players.Concat(randomPlayers).ToArray();

            await _repository.BulkInsertAsync(_players);
            await _repository.BulkInsertAsync(_players.SelectMany(p => p.Wallets).ToArray());
            await _repository.BulkInsertAsync(_players.SelectMany(p => p.Banks).ToArray());
        }

        private Player CreatePlayer(string playerCode, string username, string realName, byte[] salt,
            byte[] password, Guid vipId, PlayerType playerType, string invitationCode,
            string parentPlayerCode = null, string parentInvitationCode = null,
            Guid? brandId = null, string currencyCode = "CNY")
        {
            var defaultWalletId = GuidExtensions.NewSequentialGuid();

            brandId ??= Resellers[0].Licensees[0].Brands[0].Id;
            var brand = _brandCreated.Values.First(b => b.Id == brandId);
            var betLimitId = brand.BetLimits.Where(bl => bl.IsDefault && bl.BetLimit.CurrencyCode == currencyCode)
                .Select(bl => bl.BetLimitId).FirstOrDefault();

            return new Player
            {
                PlayerCode = playerCode,
                Username = username,
                RealName = realName,
                InvitationCode = invitationCode,
                Password = password,
                Salt = salt,
                DefaultWalletId = defaultWalletId,
                DefaultWalletCurrencyCode = currencyCode,
                Status = PlayerStatus.Enabled,
                BetLimitId = betLimitId,
                Type = playerType,
                Email = $"{Generator.RandomString(5)}@gmail.com",
                Phone = Generator.RandomIntString(10),
                QQ = Generator.RandomString(20),
                Wechat = Generator.RandomString(20),
                ParentPlayerCode = parentPlayerCode,
                ParentInvitationCode = parentInvitationCode,
                HasAgent = false,
                VipId = vipId,
                RegisteredOn = Now,
                LastActiveOn = Now,
                UpdatedOn = Now,
                UpdatedByUsername = username,
                BrandId = brand.Id,
                Wallets = new[]
                {
                    new Wallet
                    {
                        Id = defaultWalletId,
                        PlayerCode = playerCode,
                        CurrencyCode = currencyCode,
                        ExternalCurrencyCode = currencyCode,
                        Balance = 1000,
                        CreatedOn = Now,
                        UpdatedOn = Now
                    }
                },
                Banks = new[]
                {
                    new PlayerBank
                    {
                        Id = GuidExtensions.NewSequentialGuid(),
                        PlayerCode = playerCode,
                        RealName = Generator.RandomString(4),
                        BankName = Generator.RandomString(12),
                        BankAccount = Generator.RandomIntString(20)
                    }
                }
            };
        }

        private async Task GenerateGameProvidersAsync()
        {
            foreach (var gp in _gameProviders)
            {
                var gameProvider = CreateGameProvider(gp);
                _gameProvidersCreated.Add(gp.Code, gameProvider);
            }

            var gameProviders = _gameProvidersCreated.Values.ToArray();
            await _repository.BulkInsertAsync(gameProviders);
            await _repository.BulkInsertAsync(gameProviders.SelectMany(gp => gp.GameProviderConfigurations).ToArray());
            await _repository.BulkInsertAsync(gameProviders.SelectMany(gp => gp.Currencies).ToArray());
            await _repository.BulkInsertAsync(gameProviders.SelectMany(gp => gp.Languages).ToArray());
            await _repository.BulkInsertAsync(gameProviders.SelectMany(gp => gp.Games).ToArray());
            await _repository.BulkInsertAsync(gameProviders.SelectMany(gp => gp.BetLimits).ToArray());
        }

        private async Task GenerateResellersWithDescendantsAsync()
        {
            // resellers
            var resellers = Resellers.Select(CreateResellerWithLicensees).ToArray();

            await _repository.BulkInsertAsync(resellers);
            await _repository.BulkInsertAsync(resellers.SelectMany(r => r.Currencies).ToArray());
            await _repository.BulkInsertAsync(resellers.SelectMany(r => r.Languages).ToArray());
            await _repository.BulkInsertAsync(resellers.SelectMany(r => r.GameProviders).ToArray());
            await _repository.BulkInsertAsync(resellers.SelectMany(r => r.BetLimits).ToArray());

            // licensees
            var licensees = resellers.SelectMany(r => r.Licensees).ToArray();

            await _repository.BulkInsertAsync(licensees);
            await _repository.BulkInsertAsync(licensees.SelectMany(l => l.Currencies).ToArray());
            await _repository.BulkInsertAsync(licensees.SelectMany(l => l.Languages).ToArray());
            await _repository.BulkInsertAsync(licensees.SelectMany(l => l.GameProviders).ToArray());
            await _repository.BulkInsertAsync(licensees.SelectMany(l => l.BetLimits).ToArray());

            // brands
            var brands = licensees.SelectMany(l => l.Brands).ToArray();
            foreach (var b in brands) _brandCreated.Add(b.Code, b);

            await _repository.BulkInsertAsync(brands);
            await _repository.BulkInsertAsync(brands.SelectMany(b => b.Currencies).ToArray());
            await _repository.BulkInsertAsync(brands.SelectMany(b => b.Languages).ToArray());
            await _repository.BulkInsertAsync(brands.SelectMany(b => b.GameProviders).ToArray());
            await _repository.BulkInsertAsync(brands.SelectMany(b => b.Games).ToArray());
            await _repository.BulkInsertAsync(brands.SelectMany(b =>
                b.GameProviders.SelectMany(p => p.Schedules)).ToArray());
            await _repository.BulkInsertAsync(brands.SelectMany(b => b.BetLimits).ToArray());

            // brand groups
            await _repository.BulkInsertAsync(brands.SelectMany(b => b.BrandGroups).ToArray());
            await _repository.BulkInsertAsync(brands.SelectMany(b =>
                b.BrandGroups.SelectMany(g => g.BrandGames)).ToArray());
            await _repository.BulkInsertAsync(brands.SelectMany(b =>
                b.BrandGroups.SelectMany(g => g.BrandGroupBetLimits)).ToArray());
            await _repository.BulkInsertAsync(brands.SelectMany(b =>
                b.BrandGroups.SelectMany(g => g.BrandGroupBetLimits)
                    .SelectMany(g => g.GameProviderBetLimits)).ToArray());
        }

        private Task GenerateLocalizedStringsAsync()
        {
            var localizedStrings = new List<LocalizedString>();

            foreach (var gp in _gameProvidersCreated.Values)
            foreach (var cultureCode in gp.Languages.Select(l => l.CultureCode))
            {
                var gpData = GetGameProviderLocalizedStrings(gp, cultureCode);
                localizedStrings.AddRange(gpData);

                foreach (var g in gp.Games)
                {
                    var gameData = GetGameLocalizedStrings(g, cultureCode);
                    localizedStrings.AddRange(gameData);
                }
            }

            return _repository.BulkInsertAsync(localizedStrings);
        }

        private async Task GenerateSportTypesAndMarketTypesAsync()
        {
            var sports = _gameProviders.Where(g => g.SportTypes.Count > 0)
                .SelectMany(g => g.SportTypes.Select(s => new SportType
                {
                    Id = Guid.NewGuid(),
                    GameProviderId = g.Id,
                    Code = s.Key,
                    ExternalId = s.Value
                })).ToArray();

            var markets = _gameProviders.Where(g => g.MarketTypes.Count > 0)
                .SelectMany(g => g.MarketTypes.Select(s => new MarketType
                {
                    Id = Guid.NewGuid(),
                    GameProviderId = g.Id,
                    Code = s.Key,
                    ExternalId = s.Value
                })).ToArray();

            var relation = (from s in sports
                let m = markets.First(m => m.GameProviderId == s.GameProviderId)
                select new SportMarketType
                {
                    Id = Guid.NewGuid(),
                    SportTypeId = s.Id,
                    MarketTypeId = m.Id
                }).ToArray();

            await _repository.BulkInsertAsync(sports);
            await _repository.BulkInsertAsync(markets);
            await _repository.BulkInsertAsync(relation);
        }

        private Task GeneratePlayerSportsBetLimitsAsync()
        {
            var relation = _repository.SportMarketTypes.ToArray();

            var players = from p in _players.Where(p => p.Type == PlayerType.TestPlayer)
                group p by p.DefaultWalletCurrencyCode
                into g
                select g.First();

            var limits = (from p in players
                from r in relation
                select new PlayerSportsBetLimit
                {
                    PlayerCode = p.PlayerCode,
                    SportMarketTypeId = r.Id,
                    MinBet = 1,
                    MaxBet = 2000,
                    MaxBetPerMatch = 5000
                }).ToArray();

            return _repository.BulkInsertAsync(limits);
        }

        private static GameProvider CreateGameProvider(IGameProvider gameProvider)
        {
            var gp = new GameProvider
            {
                Id = gameProvider.Id,
                Name = gameProvider.Name,
                Code = gameProvider.Code,
                UniqueId = gameProvider.UniqueId,
                Description = gameProvider.Name,
                AutoCloseRound = gameProvider.AutoClosedRound,
                Status = Status.Active,
                SupportTestPlayer = gameProvider.SupportTestPlayer,
                AllowKickPlayer = gameProvider.AllowKickPlayer,
                CreatedOn = Now,
                UpdatedOn = Now,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername,
                Currencies =
                    gameProvider.Currencies.Select(curr =>
                        new GameProviderCurrency
                        {
                            GameProviderId = gameProvider.Id,
                            CurrencyCode = curr.Code,
                            GameProviderCurrencyCode = curr.ExternalCode
                        }).ToHashSet(),
                Languages = gameProvider.Languages.Select(lang =>
                    new GameProviderLanguage
                    {
                        GameProviderId = gameProvider.Id,
                        CultureCode = lang.Code,
                        GameProviderCultureCode = lang.ExternalCode
                    }).ToHashSet()
            };

            gp.GameProviderConfigurations = gameProvider.GameProviderConfigurations.Select((gpc, idx) =>
                new GameProviderConfiguration
                {
                    Id = gpc.Id,
                    Name = gpc.Name,
                    ClientId = gpc.ClientId,
                    SecretKey = gpc.SecretKey,
                    DesktopEndpoint = gpc.DesktopEndpoint,
                    MobileEndpoint = gpc.MobileEndpoint,
                    Description = gp.Name,
                    GameProviderId = gp.Id,
                    CreatedOn = Now,
                    UpdatedOn = Now,
                    CreatedByUsername = AdminUsername,
                    UpdatedByUsername = AdminUsername,
                    ConfigurationData = gpc.ConfigurationData,
                    IsDefault = idx == 0
                }).ToHashSet();

            gp.Games = gameProvider.Games.OrderBy(g => g.Code).ThenBy(g => g.PlatformType)
                .Select((g, idx) => new Models.Game
                {
                    Id = int.Parse($"{gp.UniqueId}{idx + 1:0000}"),
                    Name = g.Name,
                    ExternalId = g.ExternalId,
                    Code = g.Code,
                    EndpointPath = g.EndpointPath,
                    RuleUrl = g.GameRuleUrl,
                    Type = g.GameTypeCode,
                    Description = g.Description,
                    Status = GameStatus.Active,
                    GameProvider = gp,
                    GameProviderId = gp.Id,
                    PlatformType = g.PlatformType,
                    HasTurnoverOnTieBet = g.HasTurnoverOnTieBet,
                    SubGame = g.SubGame,
                    Html5 = g.Html5,
                    CreatedOn = Now,
                    UpdatedOn = Now,
                    CreatedByUsername = AdminUsername,
                    UpdatedByUsername = AdminUsername,
                    AvailableCurrencies = g.AvailableCurrencies
                }).ToHashSet();

            gp.BetLimits = gameProvider.BetLimits.Select(b => new Models.GameProviderBetLimit
            {
                Id = b.Id,
                GameProviderId = gp.Id,
                CurrencyCode = b.CurrencyCode,
                MinBet = b.MinBet,
                MaxBet = b.MaxBet,
                MaxBetPerMatch = b.MaxBetPerMatch,
                ExternalSetting = b.ExternalSetting,
                IsDefault = b.IsDefault
            }).ToHashSet();

            return gp;
        }

        private Reseller CreateResellerWithLicensees(IReseller reseller)
        {
            var gameProviders = GetGameProviders(reseller.GameProviders);

            return new Reseller()
            {
                Id = reseller.Id,
                Code = reseller.Code,
                Name = reseller.Name,
                OperationalEmail = reseller.OperationalEmail,
                Description = string.Empty,
                BusinessType = reseller.BusinessType,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername,
                CreatedOn = Now,
                UpdatedOn = Now,
                Currencies = reseller.Currencies.Select(curr => new ResellerCurrency
                {
                    ResellerId = reseller.Id,
                    CurrencyCode = curr.Code
                }).ToHashSet(),
                Languages = reseller.Languages.Select(lang => new ResellerLanguage
                {
                    ResellerId = reseller.Id,
                    CultureCode = lang.Code
                }).ToHashSet(),
                Licensees = reseller.Licensees.Select(l => CreateLicenseeWithBrands(reseller, l)).ToHashSet(),
                GameProviders = gameProviders.Select(gp => new ResellerGameProvider
                {
                    GameProviderId = gp.Id,
                    ResellerId = reseller.Id
                }).ToHashSet(),
                BetLimits = _betLimits.Select(bl => new ResellerBetLimit
                {
                    ResellerId = reseller.Id,
                    BetLimitId = bl.Id
                }).ToHashSet()
            };
        }

        private Licensee CreateLicenseeWithBrands(IReseller reseller, ILicensee licensee)
        {
            var gameProviderCodes = reseller.GameProviders.Length == 0
                ? licensee.GameProviders
                : reseller.GameProviders.Intersect(licensee.GameProviders).ToArray();

            var gameProviders = GetGameProviders(gameProviderCodes);

            return new Licensee()
            {
                Id = licensee.Id,
                Code = licensee.Code,
                Name = licensee.Name,
                OperationalEmail = licensee.OperationalEmail,
                Status = Status.Active,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername,
                CreatedOn = Now,
                UpdatedOn = Now,
                ResellerId = reseller.Id,
                Description = string.Empty,
                Currencies = licensee.Currencies.Select(curr => new LicenseeCurrency
                {
                    LicenseeId = licensee.Id,
                    CurrencyCode = curr.Code
                }).ToHashSet(),
                Languages = licensee.Languages.Select(lang => new LicenseeLanguage
                {
                    LicenseeId = licensee.Id,
                    CultureCode = lang.Code
                }).ToHashSet(),
                GameProviders = gameProviders.Select(gp => new LicenseeGameProvider
                {
                    GameProviderId = gp.Id,
                    LicenseeId = licensee.Id
                }).ToHashSet(),
                Brands = licensee.Brands.Select(b => CreateBrand(reseller, licensee, b)).ToHashSet(),
                BetLimits = _betLimits.Select(bl => new LicenseeBetLimit
                {
                    LicenseeId = licensee.Id,
                    BetLimitId = bl.Id
                }).ToHashSet()
            };
        }

        private Brand CreateBrand(IReseller reseller, ILicensee licensee, IBrand brand)
        {
            var gameProviderCodes = IntersectGameProviderCodes(reseller.GameProviders, licensee.GameProviders);
            gameProviderCodes = IntersectGameProviderCodes(gameProviderCodes, brand.GameProviders);
            var gameProviders = GetGameProviders(gameProviderCodes);
            var gameProviderBetLimits = gameProviders.SelectMany(gp => gp.BetLimits)
                .Where(l => l.IsDefault).ToArray();

            var brandGameProviders = gameProviders.Select(gp =>
            {
                var brandGameProviderId = Guid.NewGuid();
                return new BrandGameProvider
                {
                    Id = brandGameProviderId,
                    GameProviderId = gp.Id,
                    BrandId = brand.Id,
                    GameProviderConfigurationId = gp.GameProviderConfigurations.Where(gpc => gpc.IsDefault)
                        .Select(gpc => gpc.Id).FirstOrDefault(),
                    DefaultLanguageCode = "en-US",
                    DesktopQueryString = string.Empty,
                    MobileQueryString = string.Empty,
                    Schedules = new BrandGameProviderSchedule[]
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            BrandGameProviderId = brandGameProviderId,
                            From = DateTimeOffset.UtcNow,
                            To = DateTimeOffset.UtcNow.AddDays(7),
                            DayOfWeek = DayOfWeek.Monday,
                            IsDefault = true
                        }
                    }.ToHashSet()
                };
            });

            var games = gameProviders.SelectMany(gp => gp.Games).Select((g, idx) => new BrandGame
            {
                Id = GuidExtensions.NewSequentialGuid(),
                BrandId = brand.Id,
                GameId = g.Id,
                Status = GameStatus.Active,
                Tag = GetGameTag(idx)
            }).ToHashSet();

            var betLimits = _betLimits.Select(bl => new BrandBetLimit
            {
                BrandId = brand.Id,
                BetLimitId = bl.Id,
                BetLimit = bl,
                IsDefault = _betLimits.Where(l => l.CurrencyCode == bl.CurrencyCode).Min(l => l.MinBet) == bl.MinBet
            }).ToHashSet();

            return new Brand()
            {
                Id = brand.Id,
                Code = brand.Code,
                Name = brand.Name,
                OperationalEmail = brand.OperationalEmail,
                Status = Status.Active,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername,
                CreatedOn = Now,
                UpdatedOn = Now,
                LicenseeId = licensee.Id,
                TimeZoneId = brand.TimeZoneId,
                Description = string.Empty,
                ClientId = brand.ClientId,
                SecretKey = brand.SecretKey,
                ApiKey = brand.ApiKey,
                Currencies = brand.Currencies.Select(curr => new BrandCurrency
                {
                    BrandId = brand.Id,
                    CurrencyCode = curr.Code,
                    BrandCurrencyCode = curr.ExternalCode
                }).ToHashSet(),
                Languages = brand.Languages.Select(lang => new BrandLanguage
                {
                    BrandId = brand.Id,
                    CultureCode = lang.Code,
                    BrandCultureCode = lang.ExternalCode
                }).ToHashSet(),
                GameProviders = brandGameProviders.ToHashSet(),
                Games = games,
                BetLimits = betLimits,
                BrandGroups = brand.Groups.Select(name =>
                {
                    var brandGroupId = Guid.NewGuid();
                    return new BrandGroup
                    {
                        Id = brandGroupId,
                        BrandId = brand.Id,
                        Code = name,
                        Name = name,
                        CreatedByUsername = AdminUsername,
                        UpdatedByUsername = AdminUsername,
                        CreatedOn = Now,
                        UpdatedOn = Now,
                        Description = string.Empty,
                        BrandGames = games.Select(g => new BrandGroupGame
                        {
                            BrandGroupId = brandGroupId,
                            BrandGameId = g.Id
                        }).ToHashSet(),
                        BrandGroupBetLimits = betLimits.Select(b =>
                        {
                            var brandGroupBetLimitId = Guid.NewGuid();
                            return new BrandGroupBetLimit
                            {
                                Id = brandGroupBetLimitId,
                                BrandGroupId = brandGroupId,
                                BetLimitId = b.BetLimitId,
                                GameProviderBetLimits = gameProviderBetLimits.Select(l =>
                                    new BrandGroupGameProviderBetLimit
                                    {
                                        BrandGroupBetLimitId = brandGroupBetLimitId,
                                        GameProviderBetLimitId = l.Id
                                    }).ToHashSet()
                            };
                        }).ToHashSet()
                    };
                }).ToHashSet()
            };

            static GameTag GetGameTag(int index)
            {
                return (index % 3) switch
                {
                    0 => GameTag.New,
                    1 => GameTag.New | GameTag.Recommend,
                    2 => GameTag.New | GameTag.Recommend | GameTag.Hot,
                    _ => GameTag.None
                };
            }

            static string[] IntersectGameProviderCodes(string[] parents, string[] children)
            {
                return parents.Length == 0 ? children :
                    children.Length == 0 ? parents : parents.Intersect(children).ToArray();
            }
        }

        private static IEnumerable<LocalizedString> GetGameProviderLocalizedStrings(GameProvider gp, string cultureCode)
        {
            return new LocalizedString[]
            {
                new()
                {
                    Context = "GameProvider.Name",
                    Key = gp.Id.ToString(),
                    CultureCode = cultureCode,
                    Translation = gp.Name
                },
                new()
                {
                    Context = "GameProvider.Description",
                    Key = gp.Id.ToString(),
                    CultureCode = cultureCode,
                    Translation = gp.Description
                }
            };
        }

        private static IEnumerable<LocalizedString> GetGameLocalizedStrings(Models.Game g, string cultureCode)
        {
            return new LocalizedString[]
            {
                new()
                {
                    Context = "Game.Name",
                    Key = g.Id.ToString(),
                    CultureCode = cultureCode,
                    Translation = g.Name
                },
                new()
                {
                    Context = "Game.Description",
                    Key = g.Id.ToString(),
                    CultureCode = cultureCode,
                    Translation = g.Description
                }
            };
        }

        private static async Task LogSeedingTimeAsync(Func<Task> getFunc,
            StringBuilder builder)
        {
            var timer = Stopwatch.StartNew();
            await getFunc();
            timer.Stop();
            lock (Locker)
            {
                builder.AppendLine($"{getFunc.Method.Name} total spent {timer.ElapsedMilliseconds} ms");
            }
        }

        private Role[] CreateRoleWithMenus(IRole[] roles)
        {
            return roles.Select(role => new Role
            {
                Id = role.Id,
                Name = role.Name,
                DisplayName = role.Name,
                ResellerId = role.ResellerId,
                LicenseeId = role.LicenseeId,
                BrandId = role.BrandId,
                Description = role.Description,
                Status = RoleStatus.Enabled,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername,
                MenuSetting = _jsonSerializationProvider.SerializeToString(GetMenuSettings()),
                RoleType = role.RoleType
            }).ToArray();
        }

        private Task GenerateRolesAsync()
        {
            var roles = CreateRoleWithMenus(_roles);
            return _repository.BulkInsertAsync(roles);
        }

        private Task GenerateUsersAsync()
        {
            string[] usernames =
            {
                "hank.wu", "lester.wei", "sam.chuang", "houdini.sheng", "rico.lin"
            };

            var users = new List<User>
            {
                CreateUser("admin.disabled", UserStatus.Disabled, Roles.Admin, Departments[MarketDepartment]),
                CreateUser("admin.enabled", UserStatus.Enabled, Roles.Admin, Departments[MarketDepartment]),
                CreateUser("reseller.enabled", UserStatus.Enabled, Roles.Reseller, Departments[MarketDepartment]),
                CreateUser("reseller.disabled", UserStatus.Disabled, Roles.Reseller, Departments[MarketDepartment]),
                CreateUser("licensee.enabled", UserStatus.Enabled, Roles.Licensee, Departments[MarketDepartment]),
                CreateUser("licensee.disabled", UserStatus.Disabled, Roles.Licensee, Departments[MarketDepartment]),
                CreateUser("brand.enabled", UserStatus.Enabled, Roles.Brand, Departments[MarketDepartment]),
                CreateUser("brand.disabled", UserStatus.Disabled, Roles.Brand, Departments[MarketDepartment])
            };

            users.AddRange(usernames.Select(username =>
                CreateUser(username, UserStatus.Enabled, Roles.Admin, Departments[ProductDepartment])));

            return _repository.BulkInsertAsync(users);
        }

        private GameProvider[] GetGameProviders(string[] gameProviderCodes)
        {
            return gameProviderCodes.Length == 0
                ? _gameProvidersCreated.Values.ToArray()
                : gameProviderCodes
                    .Select(code => _gameProvidersCreated[code])
                    .ToArray();
        }
    }

    internal class Generator
    {
        private static readonly Random Random = new();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static string RandomIntString(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}