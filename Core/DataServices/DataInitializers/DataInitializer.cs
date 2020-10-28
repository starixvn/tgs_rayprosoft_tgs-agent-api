using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private static readonly object Locker = new object();
        private static readonly string AdminUsername = "admin.enabled";

        private static readonly IList<IGameProvider> GameProviders = new IGameProvider[]
        {
            new RedTigerGameProvider(), new SBOGameProvider()
        };

        private static readonly IList<IReseller> Resellers = new IReseller[]
        {
            new TgsReseller()
        };

        private readonly IDictionary<string, GameProvider> _gameProvidersCreated =
            new Dictionary<string, GameProvider>();

        private readonly IHashProvider _hashProvider;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;

        private readonly StringBuilder _logBuilder = new StringBuilder();
        private readonly ITgsRepository _repository;

        private readonly VipLevel[] _vipLevels =
        {
            new VipLevel {Id = GuidExtensions.NewSequentialGuid(), Code = "VIP 0", Name = "名称A"},
            new VipLevel {Id = GuidExtensions.NewSequentialGuid(), Code = "VIP 1", Name = "名称B"},
            new VipLevel {Id = GuidExtensions.NewSequentialGuid(), Code = "VIP 2", Name = "名称C"},
            new VipLevel {Id = GuidExtensions.NewSequentialGuid(), Code = "VIP 3", Name = "名称D"},
            new VipLevel {Id = GuidExtensions.NewSequentialGuid(), Code = "VIP 4", Name = "名称E"},
            new VipLevel {Id = GuidExtensions.NewSequentialGuid(), Code = "VIP 5", Name = "名称F"}
        };

        public DataInitializer(ITgsRepository repository)
        {
            _repository = repository;

            var config = new TgsHashConfig();
            _hashProvider = new HashProvider(config);
            _jsonSerializationProvider = new JsonSerializationProvider();
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
            await LogSeedingTimeAsync(GenerateBetLimitGroupsAsync, _logBuilder);
            await LogSeedingTimeAsync(GenerateVipLevelAsync, _logBuilder);
            await LogSeedingTimeAsync(GenerateGameProvidersAsync, _logBuilder);
            await LogSeedingTimeAsync(GenerateResellersAndLicenseesAsync, _logBuilder);
            await LogSeedingTimeAsync(GeneratePlayersAsync, _logBuilder);
            await LogSeedingTimeAsync(GenerateSettingsAsync, _logBuilder);
            await LogSeedingTimeAsync(GenerateLocalizedStringsAsync, _logBuilder);

            timer.Stop();

            _logBuilder.AppendLine($"Total Time was {timer.ElapsedMilliseconds} ms");
            Console.WriteLine(_logBuilder.ToString());
        }

        private Task GenerateCurrenciesAsync()
        {
            var currencies = new[]
                {
                    new {Code = "CNY", Name = "Yuan", NativeName = "元", Symbol = "¥"},
                    new {Code = "EUR", Name = "Euro", NativeName = "Euro", Symbol = "€"},
                    new {Code = "GBP", Name = "Pound Sterling", NativeName = "Pound Sterling", Symbol = "£"},
                    new {Code = "IDR", Name = "Indonesian rupiah", NativeName = "rupiah Indonesia", Symbol = "Rp"},
                    new {Code = "IDR_1000", Name = "Indonesian rupiah", NativeName = "rupiah Indonesia", Symbol = "Rp"},
                    new {Code = "INR", Name = "Indian Rupee", NativeName = "भारतीय रुपये", Symbol = "₹"},
                    new {Code = "JPY", Name = "Japanese Yen", NativeName = "円", Symbol = "¥"},
                    new {Code = "KRW", Name = "Korean won", NativeName = "Korean won", Symbol = "₩"},
                    new {Code = "MYR", Name = "Malaysian Ringgit", NativeName = "Malaysian Ringgit", Symbol = "RM"},
                    new {Code = "SGD", Name = "Singapore Dollar", NativeName = "Singapore Dollar", Symbol = "$"},
                    new {Code = "THB", Name = "Thai Baht", NativeName = "บาท", Symbol = "฿"},
                    new
                    {
                        Code = "USD", Name = "United States Dollar", NativeName = "United States Dollar", Symbol = "$"
                    },
                    new {Code = "VND", Name = "Vietnamese Dong", NativeName = "đồng", Symbol = "₫"},
                    new {Code = "VND_1000", Name = "Vietnamese Dong", NativeName = "đồng", Symbol = "₫"}
                }
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

        private static BetLimitGroup CreateBetLimitGroup(int id, string code, string name, string description)
        {
            return new BetLimitGroup
            {
                Id = id,
                Code = code,
                Name = name,
                Description = description,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername,
                CreatedOn = Now,
                UpdatedOn = Now
            };
        }

        private Task GenerateBetLimitGroupsAsync()
        {
            var betLimitGroups = new[]
            {
                CreateBetLimitGroup(1, "Bronze", "Bronze", "Level 1: Bronze"),
                CreateBetLimitGroup(2, "Silver", "Silver", "Level 2: Silver"),
                CreateBetLimitGroup(3, "Gold", "Gold", "Level 3: Gold"),
                CreateBetLimitGroup(4, "Platinum", "Platinum", "Level 4: Platinum"),
                CreateBetLimitGroup(5, "Diamond", "Diamond", "Level 5: Diamond")
            };

            return _repository.BulkInsertAsync(betLimitGroups);
        }

        private async Task GeneratePlayersAsync()
        {
            var rnd = new Random();
            var (salt, password) = _hashProvider.GenerateCredential("TgsP@ssw0rd");
            var playerCodes = new List<string>();
            var invitationCodes = new List<string>();

            var tmpPlayer = new[]
            {
                new
                {
                    Username = "TP01_cny_en-us", RealName = "TP01_cny_en-us", CultureCode = "en-US",
                    CurrencyCode = "CNY", Type = PlayerTypes.TgsTestPlayer
                },
                new
                {
                    Username = "TP02_cny_en-us", RealName = "TP02_cny_en-us", CultureCode = "en-US",
                    CurrencyCode = "CNY", Type = PlayerTypes.TgsTestPlayer
                },
                new
                {
                    Username = "TP01_cny_zh-cn", RealName = "TP01_cny_zh-cn", CultureCode = "zh-CN",
                    CurrencyCode = "CNY", Type = PlayerTypes.TgsTestPlayer
                },
                new
                {
                    Username = "RP01_cny_en-us", RealName = "TP01_cny_en-us", CultureCode = "en-US",
                    CurrencyCode = "CNY", Type = PlayerTypes.RealPlayer
                },
                new
                {
                    Username = "RP02_cny_en-us", RealName = "TP02_cny_en-us", CultureCode = "en-US",
                    CurrencyCode = "CNY", Type = PlayerTypes.RealPlayer
                }
            };

            var players = tmpPlayer.Select(p =>
            {
                var playerCode = PlayerCodeGenerator.NewId();
                var vipId = _vipLevels[rnd.Next(0, 6)].Id;
                var invitationCode = Generator.RandomString(6);

                return CreatePlayer(playerCode, p.Username, p.RealName, salt, password, p.CultureCode, p.CurrencyCode,
                    vipId, p.Type, invitationCode);
            }).ToArray();

            var randomPlayers = Enumerable.Range(1, 100)
                .Select(p =>
                {
                    var currencyCode = "CNY";
                    var cultureCode = "en-US";

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

                    return CreatePlayer(playerCode, username, realName, salt, password, cultureCode, currencyCode,
                        vipId,
                        PlayerTypes.RealPlayer, invitationCode, parentPlayerCode, parentInvitationCode);
                }).ToArray();

            players = players.Concat(randomPlayers).ToArray();

            await _repository.BulkInsertAsync(players);
            await _repository.BulkInsertAsync(players.SelectMany(p => p.Wallets).ToArray());
            await _repository.BulkInsertAsync(players.SelectMany(p => p.Banks).ToArray());
        }

        private static Player CreatePlayer(string playerCode, string username, string realName, byte[] salt,
            byte[] password,
            string cultureCode, string currencyCode, Guid vipId, PlayerTypes playerType, string invitationCode,
            string parentPlayerCode = null,
            string parentInvitationCode = null)
        {
            var defaultWalletId = GuidExtensions.NewSequentialGuid();

            return new Player
            {
                PlayerCode = playerCode,
                Username = username,
                RealName = realName,
                InvitationCode = invitationCode,
                Password = password,
                Salt = salt,
                CultureCode = cultureCode,
                DefaultWalletId = defaultWalletId,
                DefaultWalletCurrencyCode = currencyCode,
                Status = PlayerStatus.Enabled,
                BetLimitGroupId = 1,
                Type = playerType,
                Email = Generator.RandomString(5) + "@gmail.com",
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
                LicenseeId = Resellers[0].Licensees[0].Id,
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
                        Username = Generator.RandomString(4),
                        BankName = Generator.RandomString(12),
                        BankAccount = Generator.RandomIntString(20)
                    }
                }
            };
        }

        private async Task GenerateGameProvidersAsync()
        {
            foreach (var gp in GameProviders)
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
        }

        private async Task GenerateResellersAndLicenseesAsync()
        {
            var resellers = Resellers.Select(CreateResellerWithLicensee).ToArray();

            // resellers
            await _repository.BulkInsertAsync(resellers);
            await _repository.BulkInsertAsync(resellers.SelectMany(r => r.Currencies).ToArray());
            await _repository.BulkInsertAsync(resellers.SelectMany(r => r.Languages).ToArray());

            // licensees
            var licensees = resellers.SelectMany(r => r.Licensees).ToArray();

            await _repository.BulkInsertAsync(licensees);
            await _repository.BulkInsertAsync(licensees.SelectMany(l => l.Currencies).ToArray());
            await _repository.BulkInsertAsync(licensees.SelectMany(l => l.Languages).ToArray());
            await _repository.BulkInsertAsync(licensees.SelectMany(l => l.GameProviderConfigurations).ToArray());
            await _repository.BulkInsertAsync(licensees.SelectMany(l => l.GameProviders).ToArray());
            await _repository.BulkInsertAsync(licensees.SelectMany(l => l.Games).ToArray());
        }
        
        private Task GenerateLocalizedStringsAsync()
        {
            var localizedStrings = new List<LocalizedString>();

            foreach (var gp in _gameProvidersCreated.Values)
            {
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
            }

            return _repository.BulkInsertAsync(localizedStrings);
        }

        private static GameProvider CreateGameProvider(IGameProvider gameProvider)
        {
            var gp = new GameProvider
            {
                Id = gameProvider.Id,
                Name = gameProvider.Name,
                Code = gameProvider.Code,
                Description = gameProvider.Name,
                AutoCloseRound = gameProvider.AutoClosedRound,
                Status = Status.Active,
                SupportTestPlayer = gameProvider.SupportTestPlayer,
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

            gp.Games = gameProvider.Games.Select(g => new Models.Game
            {
                Id = g.Id,
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
                CreatedOn = Now,
                UpdatedOn = Now,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername
            }).ToHashSet();

            return gp;
        }

        private Reseller CreateResellerWithLicensee(IReseller reseller)
        {
            return new Reseller
            {
                Id = reseller.Id,
                Code = reseller.Code,
                Name = reseller.Name,
                OperationalEmail = reseller.OperationalEmail,
                Description = string.Empty,
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
                Licensees = reseller.Licensees.Select(l => CreateLicensee(reseller, l)).ToHashSet()
            };
        }

        private Licensee CreateLicensee(IReseller reseller, ILicensee licensee)
        {
            var gameProviders = licensee.GameProviders
                .Select(code => _gameProvidersCreated[code])
                .ToArray();

            return new Licensee
            {
                Id = licensee.Id,
                Code = licensee.Code,
                Name = licensee.Name,
                Status = Status.Active,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername,
                CreatedOn = Now,
                UpdatedOn = Now,
                ResellerId = reseller.Id,
                TimeZoneId = licensee.TimeZoneId,
                Description = string.Empty,
                ClientId = licensee.ClientId,
                SecretKey = licensee.SecretKey,
                Currencies = licensee.Currencies.Select(curr => new LicenseeCurrency
                {
                    LicenseeId = licensee.Id,
                    CurrencyCode = curr.Code,
                    LicenseeCurrencyCode = curr.ExternalCode
                }).ToHashSet(),
                Languages = licensee.Languages.Select(lang => new LicenseeLanguage
                {
                    LicenseeId = licensee.Id,
                    CultureCode = lang.Code,
                    LicenseeCultureCode = lang.ExternalCode
                }).ToHashSet(),
                GameProviders = gameProviders.Select(gp => new LicenseeGameProvider
                {
                    GameProviderId = gp.Id,
                    LicenseeId = licensee.Id
                }).ToHashSet(),
                Games = gameProviders.SelectMany(gp => gp.Games).Select(g => new LicenseeGame
                {
                    LicenseeId = licensee.Id,
                    GameId = g.Id
                }).ToHashSet(),
                GameProviderConfigurations = gameProviders.SelectMany(gp => gp.GameProviderConfigurations)
                    .Where(gpc => gpc.IsDefault).Select(gpc => new LicenseeGameProviderConfiguration
                    {
                        Id = Guid.NewGuid(),
                        LicenseeId = licensee.Id,
                        GameProviderId = gpc.GameProviderId,
                        GameProviderConfigurationId = gpc.Id,
                        DefaultLanguageCode = "en-US",
                        DesktopQueryString = string.Empty,
                        MobileQueryString = string.Empty
                    })
                    .ToHashSet()
            };
        }

        
        private static IEnumerable<LocalizedString> GetGameProviderLocalizedStrings(GameProvider gp, string cultureCode)
        {
            return new LocalizedString[]
            {
                new ()
                {
                    Context = "GameProvider.Name",
                    Key = gp.Id.ToString(),
                    CultureCode = cultureCode,
                    Translation = gp.Name
                },
                new ()
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
                new ()
                {
                    Context = "Game.Name",
                    Key = g.Id.ToString(),
                    CultureCode = cultureCode,
                    Translation = g.Name
                },
                new ()
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
    }

    public class Generator
    {
        private static readonly Random _random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public static string RandomIntString(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}