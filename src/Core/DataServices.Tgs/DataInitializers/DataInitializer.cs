using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using TGS.Core.DataServices.Base.DataInitializers;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Utility;
using TGS.Core.Models.Enums;
using static TGS.Core.Models.Enums.GameType;
using static TGS.Core.Models.Enums.GameActionType;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    [ExcludeFromCodeCoverage]
    public sealed partial class DataInitializer : BaseDataInitializer
    {
        private const string AdminUsername = "admin.enabled";
        private static readonly DateTimeOffset Now = DateTimeOffset.UtcNow;

        private static readonly IList<IReseller> Resellers = new IReseller[] {new TgsReseller()};

        private readonly List<Agent> _agents = new();

        private readonly IDictionary<string, Brand> _brandCreated =
            new Dictionary<string, Brand>();

        private readonly CurrencyDefined[] _currencies =
        {
            new("CNY", "Yuan", "元", "¥"), new("EUR", "Euro", "Euro", "€"),
            new("GBP", "Pound Sterling", "Pound Sterling", "£"),
            new("IDR", "Indonesian rupiah", "rupiah Indonesia", "Rp"),
            new("IDR_1000", "Indonesian rupiah", "rupiah Indonesia", "Rp"),
            new("INR", "Indian Rupee", "भारतीय रुपये", "₹"), new("JPY", "Japanese Yen", "円", "¥"),
            new("KRW", "Korean won", "Korean won", "₩"), new("MYR", "Malaysian Ringgit", "Malaysian Ringgit", "RM"),
            new("SGD", "Singapore Dollar", "Singapore Dollar", "$"), new("THB", "Thai Baht", "บาท", "฿"),
            new("USD", "United States Dollar", "United States Dollar", "$"),
            new("VND", "Vietnamese Dong", "đồng", "₫"), new("VND_1000", "Vietnamese Dong", "đồng", "₫")
        };

        private readonly IList<IGameProvider> _gameProviders;

        private readonly IDictionary<string, GameProvider> _gameProvidersCreated =
            new Dictionary<string, GameProvider>();

        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IPasswordHandler _passwordHandler;
        private readonly ITgsRepository _repository;

        private readonly IRole[] _roles =
        {
            new AdminRole(), new ResellerAdminRole(), new LicenseeAdminRole(), new BrandRole()
        };

        private BetLimit[] _betLimits;
        private Player[] _players;
        private VipLevel[] _vipLevels;

        public DataInitializer(ITgsRepository repository, IWebHostEnvironment env)
        {
            _repository = repository;

            var hashProvider = (IHashProvider)new HashProvider();
            _passwordHandler = new PasswordHandler(hashProvider);
            _jsonSerializationProvider = new JsonSerializationProvider();

            _gameProviders = new IGameProvider[]
            {
                new SBOGameProvider(env), new HabaneroGameProvider(env), new PragmaticPlayGameProvider(env), new KYGameProvider(env)
            };
        }

        protected override async Task SetInitialDataAsync()
        {
            if (_repository.Currencies.Any())
            {
                return;
            }

            var logBuilder = new StringBuilder();

            var timer = new Stopwatch();
            timer.Start();

            await LogSeedingTimeAsync(GenerateCurrenciesAsync, logBuilder);
            await LogSeedingTimeAsync(GenerateLanguagesAsync, logBuilder);
            await LogSeedingTimeAsync(GenerateBetLimitsAsync, logBuilder);
            await LogSeedingTimeAsync(GenerateGameProvidersAsync, logBuilder);
            await LogSeedingTimeAsync(GenerateResellersWithDescendantsAsync, logBuilder);
            await LogSeedingTimeAsync(GenerateVipLevelsAsync, logBuilder);
            await LogSeedingTimeAsync(GenerateAgentsAsync, logBuilder);
            await LogSeedingTimeAsync(GenerateSettingsAsync, logBuilder);
            await LogSeedingTimeAsync(GenerateLocalizedStringsAsync, logBuilder);
            await LogSeedingTimeAsync(GeneratePlayersAsync, logBuilder);
            await LogSeedingTimeAsync(GeneratePaymentAsync, logBuilder);
            await LogSeedingTimeAsync(GenerateDepartmentsAsync, logBuilder);
            await LogSeedingTimeAsync(GenerateRolesAsync, logBuilder);
            await LogSeedingTimeAsync(GenerateUsersAsync, logBuilder);
            await LogSeedingTimeAsync(GenerateSportTypesAndMarketTypesAsync, logBuilder);
            await LogSeedingTimeAsync(GeneratePlayerSportsBetLimitsAsync, logBuilder);
            await LogSeedingTimeAsync(GenerateBetDataAsync, logBuilder);
            await LogSeedingTimeAsync(GeneratePromotionsAsync, logBuilder);

            timer.Stop();

            logBuilder.AppendLine($"Total Time was {timer.ElapsedMilliseconds} ms");
            Console.WriteLine(logBuilder.ToString());
        }

        private async Task GeneratePromotionsAsync()
        {
            var now = new DateTimeOffset(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, DateTimeOffset.Now.Day, 0,
                0, 0, new(8, 0, 0));
            var promotions = _brandCreated.Select(x =>
            {
                var promotionId = Guid.NewGuid();
                return new Promotion
                {
                    Id = promotionId,
                    Name = $"{x.Value.Name}_發發發流流流長江流水向東流",
                    From = now,
                    To = now.AddDays(7),
                    Status = SwitchStatus.Enabled,
                    BrandId = x.Value.Id,
                    PromotionType = "TurnoverRebate",
                    BonusAmountLimit = 1000000,
                    Unlimited = false,
                    PromotionGameProviders =
                        _gameProviders.Select(g =>
                                new PromotionGameProvider {PromotionId = promotionId, GameProviderId = g.Id})
                            .ToHashSet(),
                    PromotionVipLevels =
                        x.Value.VipLevels.Select(v =>
                                new PromotionVipLevel {PromotionId = promotionId, VipLevelId = v.Id, Level = v.Level})
                            .ToHashSet(),
                    PromotionParticipants = x.Value.Players.Select(p => new PromotionParticipant
                    {
                        PlayerCode = p.PlayerCode, PromotionId = promotionId
                    }).ToHashSet()
                };
            });

            await _repository.Promotions.AddRangeAsync(promotions);
            await _repository.SaveChangesAsync();
        }

        private async Task GenerateCurrenciesAsync()
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

            await _repository.Currencies.AddRangeAsync(currencies);
            await _repository.SaveChangesAsync();
        }

        private async Task GenerateAgentsAsync()
        {
            var brands = Resellers.SelectMany(x => x.Licensees.SelectMany(l => l.Brands))
                .Distinct()
                .Select(brand => brand)
                .ToArray();

            foreach (var brand in brands)
            {
                var chiefId = GuidExtensions.NewSequentialGuid();
                _agents.Add(CreateAgent($"{brand.Name}Chief", AgentStatus.Enabled,
                    AgentConfigStatus.DoneForConfigurePercentage,
                    AgentLevelType.Chief,
                    AgentType.Cash, brand.Id, "CNY", chiefId));
                _agents.Add(CreateAgent($"{brand.Name}Agent1", AgentStatus.Enabled,
                    AgentConfigStatus.DoneForConfigurePercentage,
                    AgentLevelType.Agent,
                    AgentType.Cash, brand.Id, "CNY", null,
                    chiefId));
                _agents.Add(CreateAgent($"{brand.Name}Agent2", AgentStatus.Enabled,
                    AgentConfigStatus.WaitForConfigurePercentage,
                    AgentLevelType.Agent,
                    AgentType.Cash, brand.Id, "CNY", null,
                    chiefId));
            }

            await _repository.Agents.AddRangeAsync(_agents);
            await _repository.SaveChangesAsync();
        }

        private async Task GenerateVipLevelsAsync()
        {
            _vipLevels = Resellers.SelectMany(x => x.Licensees.SelectMany(l => l.Brands))
                .Distinct()
                .Select(brand => new VipLevel
                {
                    Id = GuidExtensions.NewSequentialGuid(),
                    BrandId = brand.Id,
                    Level = 1,
                    LiveDealerRebatePercentage = 1,
                    LotteryRebatePercentage = 2,
                    CardGameRebatePercentage = 3,
                    SlotRebatePercentage = 4,
                    SportsRebatePercentage = 5,
                    VirtualSportsRebatePercentage = 6,
                    CreatedByUsername = AdminUsername,
                    UpdatedByUsername = AdminUsername,
                    CreatedOn = Now,
                    UpdatedOn = Now
                })
                .ToArray();

            await _repository.VipLevels.AddRangeAsync(_vipLevels);
            await _repository.SaveChangesAsync();
        }

        private async Task GenerateLanguagesAsync()
        {
            var languages = new[]
                {
                    new {CultureCode = "en-US", LanguageCode = "en", Name = "English", LocalizedName = "English"},
                    new
                    {
                        CultureCode = "id-ID",
                        LanguageCode = "id",
                        Name = "Indonesian",
                        LocalizedName = "bahasa Indonesia"
                    },
                    new {CultureCode = "ja-JP", LanguageCode = "ja", Name = "Japanese", LocalizedName = "日本語"},
                    new {CultureCode = "ko-KR", LanguageCode = "ko", Name = "Korean", LocalizedName = "한국어 / 조선말"},
                    new {CultureCode = "th-TH", LanguageCode = "th", Name = "Thai", LocalizedName = "ภาษาไทย"},
                    new
                    {
                        CultureCode = "vi-VN",
                        LanguageCode = "vi",
                        Name = "Vietnamese",
                        LocalizedName = "ngôn ngữ tiếng Việt"
                    },
                    new
                    {
                        CultureCode = "zh-CN",
                        LanguageCode = "cn",
                        Name = "Simplified Chinese",
                        LocalizedName = "简体中文"
                    },
                    new
                    {
                        CultureCode = "zh-TW",
                        LanguageCode = "zh",
                        Name = "Traditional Chinese",
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

            await _repository.Languages.AddRangeAsync(languages);
            await _repository.SaveChangesAsync();
        }

        private async Task GenerateBetLimitsAsync()
        {
            BetLimitDefined[] limits =
            {
                new("CNY", 1, 200, 1000), new("CNY", 10, 300, 1000), new("EUR", 1, 200, 1000),
                new("GBP", 1, 200, 1000), new("IDR", 1000, 200000, 1000000), new("IDR_1000", 1, 200, 1000),
                new("INR", 1, 200, 1000), new("JPY", 1, 200, 1000), new("KRW", 1, 200, 1000),
                new("MYR", 1, 200, 1000), new("SGD", 1, 200, 1000), new("THB", 1, 200, 1000),
                new("USD", 1, 200, 1000), new("VND", 1000, 200000, 1000000), new("VND_1000", 1, 200, 1000)
            };

            _betLimits = limits.Select(b => new BetLimit
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

            await _repository.BetLimits.AddRangeAsync(_betLimits);
            await _repository.SaveChangesAsync();
        }

        private async Task GeneratePlayersAsync()
        {
            var playerDefined = new PlayerDefined[]
            {
                new("4ucmqplwxqlv", new("2EA6CDEF-094F-EB11-8D88-70B5E82B6B8D"),
                    new("D71AC5ED-B7D9-49F2-A129-8709FA4CC02A"),
                    _agents[0].Id),
                new("4ucmqplx11cs", new("30A6CDEF-094F-EB11-8D88-70B5E82B6B8D"),
                    new("D71AC5ED-B7D9-49F2-A129-8709FA4CC02A"),
                    _agents[0].Id),
                new("4ucmqplx4dti", new("32A6CDEF-094F-EB11-8D88-70B5E82B6B8D"),
                    new("D71AC5ED-B7D9-49F2-A129-8709FA4CC02A"),
                    _agents[1].Id),
                new("4ucmqplx7okb", new("34A6CDEF-094F-EB11-8D88-70B5E82B6B8D"),
                    new("D71AC5ED-B7D9-49F2-A129-8709FA4CC02A"), _agents[2].Id),
                new("4ucmqplxb029", new("36A6CDEF-094F-EB11-8D88-70B5E82B6B8D"),
                    new("4E86719F-243F-46F6-B010-5C758B7285AF"), null),
                new("4ucmqplxe65x", new("38A6CDEF-094F-EB11-8D88-70B5E82B6B8D"),
                    new("4E86719F-243F-46F6-B010-5C758B7285AF"), null),
                new("4ucmqplxhfoe", new("3AA6CDEF-094F-EB11-8D88-70B5E82B6B8D"),
                    new("4E86719F-243F-46F6-B010-5C758B7285AF"), null),
                new("4ucmqplxkuyd", new("3CA6CDEF-094F-EB11-8D88-70B5E82B6B8D"),
                    new("4E86719F-243F-46F6-B010-5C758B7285AF"), null)
            };

            var (salt, password) = _passwordHandler.GenerateCredential("TgsP@ssw0rd");

            _players = playerDefined.Select(p =>
            {
                var playerCode = p.PlayerCode;
                var brandId = p.BrandId;
                var username = p.PlayerCode;
                var realName = Generator.RandomString(12);
                var vipId = _vipLevels.First(x => x.BrandId == brandId).Id;

                return CreatePlayer(playerCode, brandId, p.WalletId, username, realName, salt, password, vipId,
                    PlayerType.RealPlayer, p.AgentId);
            }).ToArray();

            await _repository.Players.AddRangeAsync(_players);
            await _repository.SaveChangesAsync();
        }

        private Player CreatePlayer(string playerCode,
            Guid brandId, Guid walletId, string username, string realName, byte[] salt,
            byte[] password, Guid vipId, PlayerType playerType,
            Guid? agentId, string currencyCode = "CNY")
        {
            var brand = _brandCreated.Values.First(b => b.Id == brandId);
            var betLimitId = brand.BetLimits.Where(bl => bl.IsDefault && bl.BetLimit.CurrencyCode == currencyCode)
                .Select(bl => bl.BetLimitId).FirstOrDefault();

            return new()
            {
                PlayerCode = playerCode,
                Username = username,
                RealName = realName,
                Password = password,
                Salt = salt,
                WithdrawPassword = Array.Empty<byte>(),
                WithdrawSalt = Array.Empty<byte>(),
                DefaultWalletId = walletId,
                DefaultWalletCurrencyCode = currencyCode,
                Status = PlayerStatus.Enabled,
                BetLimitId = betLimitId,
                Type = playerType,
                Email = $"{Generator.RandomString(5)}@gmail.com",
                Phone = Generator.RandomIntString(10),
                QQ = Generator.RandomString(20),
                Wechat = Generator.RandomString(20),
                VipId = vipId,
                AgentId = agentId,
                RegisteredOn = Now,
                LastActiveOn = Now,
                UpdatedOn = Now,
                UpdatedByUsername = username,
                BrandId = brand.Id,
                Wallets =
                    new[]
                    {
                        new Wallet
                        {
                            Id = walletId,
                            PlayerCode = playerCode,
                            CurrencyCode = currencyCode,
                            ExternalCurrencyCode = currencyCode,
                            Balance = 1000,
                            CreatedOn = Now,
                            UpdatedOn = Now
                        }
                    }
                //Banks = new[]
                //{
                //    new PlayerBankInfo
                //    {
                //        Id = GuidExtensions.NewSequentialGuid(),
                //        PlayerCode = playerCode,
                //        RealName = Generator.RandomString(4),
                //        BankName = Generator.RandomString(12),
                //        BankAccount = Generator.RandomIntString(20),
                //        CreatedByUsername = AdminUsername,
                //        UpdatedByUsername = AdminUsername,
                //        CreatedOn = Now,
                //        UpdatedOn = Now
                //    }
                //}
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

            await _repository.GameProviders.AddRangeAsync(gameProviders);
            await _repository.SaveChangesAsync();
        }

        private async Task GenerateResellersWithDescendantsAsync()
        {
            // resellers
            var resellers = Resellers.Select(CreateResellerWithLicensees).ToArray();

            await _repository.Resellers.AddRangeAsync(resellers);

            // licensees
            var licensees = resellers.SelectMany(r => r.Licensees).ToArray();
            await _repository.Licensees.AddRangeAsync(licensees);

            // brands
            var brands = licensees.SelectMany(l => l.Brands).ToArray();
            foreach (var b in brands)
            {
                _brandCreated.Add(b.Code, b);
            }

            await _repository.Brands.AddRangeAsync(brands);
            await _repository.SaveChangesAsync();
        }

        private async Task GenerateLocalizedStringsAsync()
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

            await _repository.LocalizedStrings.AddRangeAsync(localizedStrings);
            await _repository.SaveChangesAsync();
        }

        private async Task GenerateSportTypesAndMarketTypesAsync()
        {
            var sports = _gameProviders.Where(g => g.SportTypes.Count > 0)
                .SelectMany(g => g.SportTypes.Select(s => new SportType
                {
                    Id = Guid.NewGuid(), GameProviderId = g.Id, Code = s.Key, ExternalId = s.Value
                })).ToArray();

            var markets = _gameProviders.Where(g => g.MarketTypes.Count > 0)
                .SelectMany(g => g.MarketTypes.Select(s => new MarketType
                {
                    Id = Guid.NewGuid(), GameProviderId = g.Id, Code = s.Key, ExternalId = s.Value
                })).ToArray();

            var relation = (from s in sports
                            let m = markets.First(m => m.GameProviderId == s.GameProviderId)
                            select new SportMarketType {Id = Guid.NewGuid(), SportTypeId = s.Id, MarketTypeId = m.Id})
                .ToArray();

            await _repository.SportTypes.AddRangeAsync(sports);
            await _repository.MarketTypes.AddRangeAsync(markets);
            await _repository.SportMarketTypes.AddRangeAsync(relation);
            await _repository.SaveChangesAsync();
        }

        private async Task GeneratePlayerSportsBetLimitsAsync()
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


            await _repository.PlayerSportsBetLimits.AddRangeAsync(limits);
            await _repository.SaveChangesAsync();
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

            gp.Games = gameProvider.Games.Select((g, idx) => new Models.Game
            {
                Id = int.Parse($"{gp.UniqueId}{idx + 1:00000}"),
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
                AvailableToBrand = g.AvailableToBrand,
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
                ExternalSetting = b.ExternalSetting
            }).ToHashSet();

            return gp;
        }

        private Reseller CreateResellerWithLicensees(IReseller reseller)
        {
            var gameProviders = GetGameProviders(reseller.GameProviders);

            return new()
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
                Currencies =
                    reseller.Currencies.Select(curr =>
                        new ResellerCurrency {ResellerId = reseller.Id, CurrencyCode = curr.Code}).ToHashSet(),
                Languages =
                    reseller.Languages.Select(lang =>
                        new ResellerLanguage {ResellerId = reseller.Id, CultureCode = lang.Code}).ToHashSet(),
                Licensees = reseller.Licensees.Select(l => CreateLicenseeWithBrands(reseller, l)).ToHashSet(),
                GameProviders =
                    gameProviders.Select(gp =>
                        new ResellerGameProvider {GameProviderId = gp.Id, ResellerId = reseller.Id}).ToHashSet(),
                BetLimits = _betLimits.Select(bl => new ResellerBetLimit {ResellerId = reseller.Id, BetLimitId = bl.Id})
                    .ToHashSet()
            };
        }

        private Licensee CreateLicenseeWithBrands(IReseller reseller, ILicensee licensee)
        {
            var gameProviderCodes = reseller.GameProviders.Length == 0
                ? licensee.GameProviders
                : reseller.GameProviders.Intersect(licensee.GameProviders).ToArray();

            var gameProviders = GetGameProviders(gameProviderCodes);

            return new()
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
                Currencies =
                    licensee.Currencies.Select(curr =>
                        new LicenseeCurrency {LicenseeId = licensee.Id, CurrencyCode = curr.Code}).ToHashSet(),
                Languages =
                    licensee.Languages.Select(lang =>
                        new LicenseeLanguage {LicenseeId = licensee.Id, CultureCode = lang.Code}).ToHashSet(),
                GameProviders =
                    gameProviders.Select(gp =>
                        new LicenseeGameProvider {GameProviderId = gp.Id, LicenseeId = licensee.Id}).ToHashSet(),
                Brands = licensee.Brands.Select(b => CreateBrand(reseller, licensee, b)).ToHashSet(),
                BetLimits = _betLimits.Select(bl => new LicenseeBetLimit {LicenseeId = licensee.Id, BetLimitId = bl.Id})
                    .ToHashSet()
            };
        }

        private Brand CreateBrand(IReseller reseller, ILicensee licensee, IBrand brand)
        {
            var gameProviderCodes = IntersectGameProviderCodes(reseller.GameProviders, licensee.GameProviders);
            gameProviderCodes = IntersectGameProviderCodes(gameProviderCodes, brand.GameProviders);
            var gameProviders = GetGameProviders(gameProviderCodes);
            var gameProviderBetLimits = gameProviders.SelectMany(gp => gp.BetLimits).ToArray();

            var brandGameProviders = gameProviders.Select(gp =>
            {
                var brandGameProviderId = Guid.NewGuid();
                return new BrandGameProvider
                {
                    Id = brandGameProviderId,
                    GameProviderId = gp.Id,
                    GameProvider = gp,
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
            }).ToArray();

            var games = gameProviders.SelectMany(gp => gp.Games)
                .Where(g => g.AvailableToBrand).Select((g, idx) => new BrandGame
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

            var (currentFromDate, currentToDate, nextFromDate, nextToDate) =
                BrandSettlementIntervalGenerator.GetInterval(brand.Type);

            return new()
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
                SettlementIntervalType = brand.Type,
                CurrentSettlementFrom = currentFromDate,
                CurrentSettlementTo = currentToDate,
                NextSettlementFrom = nextFromDate,
                NextSettlementTo = nextToDate,
                AutoSentAgentBonus = true,
                AutoSentPlayerBonus = true,
                Currencies =
                    brand.Currencies.Select(curr => new BrandCurrency
                    {
                        BrandId = brand.Id, CurrencyCode = curr.Code, BrandCurrencyCode = curr.ExternalCode
                    }).ToHashSet(),
                Languages = brand.Languages.Select(lang => new BrandLanguage
                {
                    BrandId = brand.Id, CultureCode = lang.Code, BrandCultureCode = lang.ExternalCode
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
                        Name = name,
                        CreatedByUsername = AdminUsername,
                        UpdatedByUsername = AdminUsername,
                        CreatedOn = Now,
                        UpdatedOn = Now,
                        Description = string.Empty,
                        BrandGames =
                            games.Select(g => new BrandGroupGame {BrandGroupId = brandGroupId, BrandGameId = g.Id})
                                .ToHashSet(),
                        BrandGroupBetLimits = betLimits.Select(b =>
                        {
                            var brandGroupBetLimitId = Guid.NewGuid();
                            return new BrandGroupBetLimit
                            {
                                Id = brandGroupBetLimitId,
                                BrandGroupId = brandGroupId,
                                BetLimitId = b.BetLimitId,
                                GameProviderBetLimits = brandGameProviders.Where(g =>
                                    g.GameProvider.BetLimits
                                        .Any(gb =>
                                            gb.CurrencyCode == b.BetLimit.CurrencyCode &&
                                            b.BetLimit.MinBet <= gb.MinBet &&
                                            b.BetLimit.MaxBet >= gb.MaxBet)).Select(g =>
                                {
                                    var gpBetLimits = g.GameProvider.BetLimits
                                        .OrderBy(gb => gb.MinBet).ToArray();
                                    if (gpBetLimits.Length == 0)
                                    {
                                        return null;
                                    }

                                    return new BrandGroupGameProviderBetLimit
                                    {
                                        BrandGroupBetLimitId = brandGroupBetLimitId,
                                        GameProviderBetLimitId = gpBetLimits.First().Id
                                    };
                                }).ToHashSet()
                            };
                        }).ToHashSet()
                    };
                }).ToHashSet()
            };

            static GameTag GetGameTag(int index) =>
                (index % 3) switch
                {
                    0 => GameTag.New,
                    1 => GameTag.New | GameTag.Recommend,
                    2 => GameTag.New | GameTag.Recommend | GameTag.Hot,
                    _ => GameTag.None
                };

            static string[] IntersectGameProviderCodes(string[] parents, string[] children) =>
                parents.Length == 0  ? children :
                children.Length == 0 ? parents : parents.Intersect(children).ToArray();
        }

        private static IEnumerable<LocalizedString>
            GetGameProviderLocalizedStrings(GameProvider gp, string cultureCode) =>
            new LocalizedString[]
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

        private static IEnumerable<LocalizedString> GetGameLocalizedStrings(Models.Game g, string cultureCode) =>
            new LocalizedString[]
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

        private Role[] CreateRoleWithMenus(IRole[] roles) =>
            roles.Select(role => new Role
            {
                Id = role.Id,
                IsRoot = role.IsRoot,
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

        private async Task GenerateRolesAsync()
        {
            var roles = CreateRoleWithMenus(_roles);

            await _repository.Roles.AddRangeAsync(roles);
            await _repository.SaveChangesAsync();
        }

        private async Task GenerateUsersAsync()
        {
            string[] usernames = {"hank.wu", "lester.wei", "sam.chuang", "houdini.sheng", "rico.lin"};

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

            await _repository.Users.AddRangeAsync(users);
            await _repository.SaveChangesAsync();
        }

        private async Task GenerateBetDataAsync()
        {
            var rand = new Random((int)(DateTime.UtcNow.Ticks % 1000000));
            var timestamp = DateTimeOffset.UtcNow.AddDays(-2);

            var players = (from p in _players
                           group p by p.BrandId
                           into g
                           select g.Take(2)).SelectMany(p => p);

            foreach (var p in players)
            foreach (var gp in _gameProvidersCreated.Values.Take(3))
            {
                var games = (from gm in gp.Games
                             where !gm.Type.ToString().Contains("Lobby")
                             group gm by gm.Type
                             into g
                             select g.Take(1)).SelectMany(g => g);

                foreach (var game in games)
                {
                    var createdOn = timestamp = timestamp.AddSeconds(1);
                    var closedOn = game.Type == Sportsbook || game.Type == VirtualSports
                        ? timestamp.AddDays(1)
                        : timestamp.AddSeconds(3);

                    var gameProviderConfiguration = gp.GameProviderConfigurations.First();
                    var round = CreateRound(p, gp, gameProviderConfiguration, game, createdOn, closedOn);

                    var externalGameActionGroupId = round.ExternalRoundId;
                    var betAmount = ((decimal)rand.Next(5, 20)).Negate();
                    var placeBetAction = CreateGameAction(betAmount, round.Id,
                        PlaceBet, externalGameActionGroupId, createdOn, createdOn);

                    await _repository.GameActions.AddAsync(placeBetAction);

                    var v = rand.NextDouble();
                    var resultActionType = v > 0.5 ? LoseBet : v > 0.2 && v < 0.5 ? WinBet : CancelTransaction;

                    var resultAmount = resultActionType switch
                    {
                        LoseBet           => 0,
                        CancelTransaction => betAmount.Negate(),
                        _                 => betAmount.Negate() * 2
                    };

                    decimal turnover = 0, winloss = 0;
                    if (resultActionType != CancelTransaction)
                    {
                        turnover = betAmount.Negate();
                        winloss = (betAmount + resultAmount).Negate();

                        round.WinLoss = winloss;
                    }

                    var resultAction = CreateGameAction(resultAmount, round.Id,
                        resultActionType, externalGameActionGroupId, closedOn, closedOn, turnover, winloss,
                        placeBetAction.Id);

                    await _repository.GameActions.AddAsync(resultAction);
                    await _repository.Rounds.AddAsync(round);
                }
            }

            await _repository.SaveChangesAsync();
        }

        private static Round CreateRound(Player player, GameProvider gameProvider,
            GameProviderConfiguration configuration, Models.Game game, DateTimeOffset createdOn,
            DateTimeOffset closedOn)
        {
            var roundId = Guid.NewGuid();

            return new()
            {
                Id = roundId,
                ExternalRoundId = "EXT_" + roundId,
                PlayerCode = player.PlayerCode,
                GameProviderId = gameProvider.Id,
                GameId = game.Id,
                GameProviderConfigurationId = configuration.Id,
                WalletId = player.DefaultWalletId,
                CurrencyCode = player.DefaultWalletCurrencyCode,
                Status = RoundStatus.Closed,
                BrandId = player.BrandId,
                CreatedOn = createdOn,
                ClosedOn = closedOn,
                ExternalCreatedOn = createdOn,
                ExternalClosedOn = closedOn
            };
        }

        private static GameAction CreateGameAction(decimal amount,
            Guid roundId, GameActionType gameActionType, string externalGameActionGroupId, DateTimeOffset recordedOn,
            DateTimeOffset sentOn, decimal turnover = 0, decimal winloss = 0, Guid? refTransactionId = null,
            Guid? batchId = null)
        {
            var transactionId = Guid.NewGuid();
            var unsettledBets = gameActionType == PlaceBet ? amount : 0;

            return new()
            {
                Id = transactionId,
                ExternalTransactionId = $"EXT_{transactionId}",
                Type = gameActionType,
                RoundId = roundId,
                Turnover = turnover,
                WinLoss = winloss,
                UnsettledBets = unsettledBets,
                GameActionBatchId = batchId,
                RecordedOn = recordedOn,
                SentOn = sentOn,
                PlayerIp = "127.0.0.1",
                ExternalGameActionGroupId = externalGameActionGroupId,
                ExternalBetId = $"BET_{transactionId}",
                ReferencedGameActionId = refTransactionId,
                Amount = amount
            };
        }

        private GameProvider[] GetGameProviders(IReadOnlyCollection<string> gameProviderCodes) =>
            gameProviderCodes.Count == 0
                ? _gameProvidersCreated.Values.ToArray()
                : gameProviderCodes
                    .Select(code => _gameProvidersCreated[code])
                    .ToArray();
    }

    internal class Generator
    {
        private static readonly Random Random = new();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuwxyz0123456789";
            return new(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static string RandomIntString(int length)
        {
            const string chars = "0123456789";
            return new(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}