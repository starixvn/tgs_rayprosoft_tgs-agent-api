using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Utility;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.DataInitializers
{
    public interface IDataInitializer
    {
        void SetInitialData();
    }

    [ExcludeFromCodeCoverage]
    public sealed partial class DataInitializer : IDataInitializer
    {
        private readonly ITgsRepository _repository;
        private readonly IHashProvider _hashProvider;
        private readonly IServiceLog<DataInitializer> _log;
        private readonly DateTimeOffset _now = DateTimeOffset.UtcNow;

        private readonly StringBuilder _logBuilder = new StringBuilder();

        private static readonly object Locker = new object();
        private static readonly string AdminUsername = "admin.enabled";

        private static readonly IList<IGameProvider> GameProviders = new[]
        {
            new RedTigerGameProvider()
        };

        private static readonly IList<IReseller> Resellers = new[] 
        {
            new TgsReseller()
        };

        private IDictionary<string, GameProvider> _gameProvidersCreated = new Dictionary<string, GameProvider>();

        public DataInitializer(ITgsRepository repository, IHashProvider hashProvider, IServiceLog<DataInitializer> log)
        {
            _repository = repository;
            _hashProvider = hashProvider;
            _log = log;
        }

        void IDataInitializer.SetInitialData()
        {
            if (_repository.Players.Any())
            {
                return;
            }

            var timer = new Stopwatch();
            timer.Start();
                        
            LogSeedingTime(GenerateCurrencies, _logBuilder);
            LogSeedingTime(GenerateLanguages, _logBuilder);
            LogSeedingTime(GenerateBetLimitGroups, _logBuilder);
            LogSeedingTime(GenerateVipLevel, _logBuilder);            
            LogSeedingTime(GenerateGameProviders, _logBuilder);
            LogSeedingTime(GenerateResellersAndLicensees, _logBuilder);
            LogSeedingTime(GeneratePlayers, _logBuilder);

            timer.Stop();

            _logBuilder.AppendLine($"Total Time was {timer.ElapsedMilliseconds} ms");
            _log.LogInformation(_logBuilder.ToString());
        }
      
        private void GenerateCurrencies()
        {
            var currencies = new[]
            {
                new { Code = "CNY", Name = "Yuan", NativeName = "元", Symbol ="¥" },
                new { Code = "EUR", Name = "Euro", NativeName = "Euro", Symbol ="€" },
                new { Code = "GBP", Name = "Pound Sterling", NativeName = "Pound Sterling", Symbol ="£" },
                new { Code = "IDR", Name = "Indonesian rupiah", NativeName = "rupiah Indonesia", Symbol ="Rp" },
                new { Code = "IDR_1000", Name = "Indonesian rupiah", NativeName = "rupiah Indonesia", Symbol ="Rp" },
                new { Code = "INR", Name = "Indian Rupee", NativeName = "भारतीय रुपये", Symbol ="₹" },
                new { Code = "JPY", Name = "Japanese Yen", NativeName = "円", Symbol ="¥" },
                new { Code = "KRW", Name = "Korean won", NativeName = "Korean won", Symbol ="₩" },
                new { Code = "MYR", Name = "Malaysian Ringgit", NativeName = "Malaysian Ringgit", Symbol ="RM" },
                new { Code = "SGD", Name = "Singapore Dollar", NativeName = "Singapore Dollar", Symbol ="$" },
                new { Code = "THB", Name = "Thai Baht", NativeName = "บาท",Symbol ="฿" },
                new { Code = "USD", Name = "United States Dollar", NativeName = "United States Dollar", Symbol ="$" },
                new { Code = "VND", Name = "Vietnamese Dong", NativeName ="đồng", Symbol ="₫" },
                new { Code = "VND_1000", Name = "Vietnamese Dong", NativeName ="đồng", Symbol ="₫" }
            }
            .Select(c => new Currency
            {
                Code = c.Code,
                Name = c.Name,
                NativeName = c.NativeName,
                Symbol = c.Symbol,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername,
                CreatedOn = _now,
                UpdatedOn = _now
            }).ToArray();

            _repository.BulkInsert(currencies);
        }

        private readonly VipLevel[] _vipLevels = new[]
        {
            new VipLevel { Id = GuidExtensions.NewSequentialGuid(), Code = "VIP 0", Name = "名称A" },
            new VipLevel { Id = GuidExtensions.NewSequentialGuid(), Code = "VIP 1", Name = "名称B" },
            new VipLevel { Id = GuidExtensions.NewSequentialGuid(), Code = "VIP 2", Name = "名称C" },
            new VipLevel { Id = GuidExtensions.NewSequentialGuid(), Code = "VIP 3", Name = "名称D" },
            new VipLevel { Id = GuidExtensions.NewSequentialGuid(), Code = "VIP 4", Name = "名称E" },
            new VipLevel { Id = GuidExtensions.NewSequentialGuid(), Code = "VIP 5", Name = "名称F" }
        };

        private void GenerateVipLevel()
        {
            foreach (var vip in _vipLevels)
            {
                vip.CreatedByUsername = AdminUsername;
                vip.UpdatedByUsername = AdminUsername;
                vip.CreatedOn = _now;
                vip.UpdatedOn = _now;
            }

            _repository.BulkInsert(_vipLevels);
        }

        private void GenerateLanguages()
        {
            var languages = new[]
            {
                new { CultureCode = "en-US", LanguageCode = "en", Name = "English", LocalizedName = "English" },
                new { CultureCode = "id-ID", LanguageCode = "id", Name ="Indonesian", LocalizedName ="bahasa Indonesia" },
                new { CultureCode = "ja-JP", LanguageCode = "ja", Name ="Japanese", LocalizedName ="日本語" },
                new { CultureCode = "ko-KR", LanguageCode = "ko", Name ="Korean", LocalizedName ="한국어 / 조선말" },
                new { CultureCode = "th-TH", LanguageCode = "th", Name ="Thai", LocalizedName ="ภาษาไทย" },
                new { CultureCode = "vi-VN", LanguageCode = "vi", Name ="Vietnamese", LocalizedName ="ngôn ngữ tiếng Việt" },
                new { CultureCode = "zh-CN", LanguageCode = "zh", Name ="Simplified Chinese", LocalizedName = "简体中文" },
                new { CultureCode = "zh-TW", LanguageCode = "zh", Name ="Traditional Chinese", LocalizedName =" 繁體中文" }
            }
            .Select(p => new Language
            {
                CultureCode = p.CultureCode,
                LanguageCode = p.LanguageCode,
                Name = p.Name,
                LocalizedName = p.LocalizedName,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername,
                CreatedOn = _now,
                UpdatedOn = _now
            }).ToArray();

            _repository.BulkInsert(languages);
        }

        private BetLimitGroup CreateBetLimitGroup(int id, string code, string name, string description)
        {
            return new BetLimitGroup
            {
                Id = id,
                Code = code,
                Name = name,
                Description = description,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername,
                CreatedOn = _now,
                UpdatedOn = _now
            };
        }

        private void GenerateBetLimitGroups()
        {
            var betLimitGroups = new[]
            {
                CreateBetLimitGroup(1, "Bronze", "Bronze", "Level 1: Bronze"),
                CreateBetLimitGroup(2, "Silver", "Silver", "Level 2: Silver"),
                CreateBetLimitGroup(3, "Gold", "Gold", "Level 3: Gold"),
                CreateBetLimitGroup(4, "Platinum", "Platinum", "Level 4: Platinum"),
                CreateBetLimitGroup(5, "Diamond", "Diamond", "Level 5: Diamond")
            };

            _repository.BulkInsert(betLimitGroups);
        }

        private void GeneratePlayers()
        {
            var rnd = new Random();
            var (password, salt) = _hashProvider.GenerateCredential("TgsP@ssw0rd");
            var playerCodes = new List<string>();
            var invitationCodes = new List<string>();
           
            var players = Enumerable.Range(1, 200)
                .Select(p =>
                {
                    var defaultWalletId = GuidExtensions.NewSequentialGuid();
                    var currencyCode = "CNY";

                    var playerCode = PlayerCodeGenerator.NewId();
                    var parentPlayerCode = playerCodes.Count == 0 ? null : rnd.NextDouble() > 0.5 ? null : playerCodes[rnd.Next(0, playerCodes.Count - 1)];
                    
                    var invitationCode = Generator.RandomString(6);
                    var parentInvitationCode = invitationCodes.Count == 0 ? null : rnd.NextDouble() > 0.5 ? null : invitationCodes[rnd.Next(0, invitationCodes.Count - 1)];

                    playerCodes.Add(playerCode);
                    invitationCodes.Add(invitationCode);

                    return new Player
                    {
                        PlayerCode = playerCode,
                        Username = Generator.RandomString(12),
                        RealName = Generator.RandomString(12),
                        InvitationCode = invitationCode,
                        Password = password,
                        Salt = salt,
                        CultureCode = "en-US",
                        DefaultWalletId = defaultWalletId,
                        DefaultWalletCurrencyCode = currencyCode,
                        Status = (PlayerStatus)rnd.Next(0, 3),
                        BetLimitGroupId = rnd.Next(1, 6),
                        Type = PlayerTypes.RealPlayer,
                        Email = Generator.RandomString(5) + "@gmail.com",
                        Phone = Generator.RandomIntString(10),
                        QQ = Generator.RandomString(20),
                        Wechat = Generator.RandomString(20),
                        ParentPlayerCode = parentPlayerCode,
                        ParentInvitationCode = parentInvitationCode,
                        HasAgent = rnd.NextDouble() > 0.5,
                        VipId = _vipLevels[rnd.Next(0, 6)].Id,
                        RegisteredOn = _now,
                        LastActiveOn = _now,
                        LicenseeId = Resellers[0].Licensees[0].Id,
                        Wallets = new[]
                        {
                            new Wallet
                            {
                                Id = defaultWalletId,
                                PlayerCode = playerCode,
                                CurrencyCode = currencyCode,
                                ExternalCurrencyCode = currencyCode,
                                Balance = rnd.Next(1000,100000),
                                CreatedOn = _now,
                                UpdatedOn = _now
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
                                BankAccount = Generator.RandomIntString(20),
                            },
                        }
                    };
                }).ToArray();

            _repository.BulkInsert(players);
            _repository.BulkInsert(players.SelectMany(p => p.Wallets).ToArray());
            _repository.BulkInsert(players.SelectMany(p => p.Banks).ToArray());
        }

        private void GenerateGameProviders()
        {
            foreach (var gp in GameProviders)
            {
                var gameProvider = CreateGameProvider(gp);
                _gameProvidersCreated.Add(gp.Code, gameProvider);
            }

            var gameProviders = _gameProvidersCreated.Values.ToArray();
            _repository.BulkInsert(gameProviders);
            _repository.BulkInsert(gameProviders.SelectMany(gp => gp.GameProviderConfigurations).ToArray());
            _repository.BulkInsert(gameProviders.SelectMany(gp => gp.Currencies).ToArray());
            _repository.BulkInsert(gameProviders.SelectMany(gp => gp.Languages).ToArray());
            _repository.BulkInsert(gameProviders.SelectMany(gp => gp.Games).ToArray());
        }

        private void GenerateResellersAndLicensees()
        {
            var resellers = Resellers.Select(r => CreateResellerWithLicensee(r)).ToArray();

            // resellers
            _repository.BulkInsert(resellers);
            _repository.BulkInsert(resellers.SelectMany(r => r.Currencies).ToArray());
            _repository.BulkInsert(resellers.SelectMany(r => r.Languages).ToArray());

            // licensees
            var licensees = resellers.SelectMany(r => r.Licensees).ToArray();
            
            _repository.BulkInsert(licensees);
            _repository.BulkInsert(licensees.SelectMany(l => l.Currencies).ToArray());
            _repository.BulkInsert(licensees.SelectMany(l => l.Languages).ToArray());
            _repository.BulkInsert(licensees.SelectMany(l => l.GameProviderConfigurations).ToArray());
            _repository.BulkInsert(licensees.SelectMany(l => l.Games).ToArray());
        }

        private GameProvider CreateGameProvider(IGameProvider gameProvider)
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
                CreatedOn = _now,
                UpdatedOn = _now,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername,
                Currencies =
                    gameProvider.Currencies.Select(curr =>
                        new GameProviderCurrency
                        {
                            GameProviderId = gameProvider.Id,
                            CurrencyCode = curr.Code,
                            GameProviderCurrencyCode = curr.ExternalCode,
                        }).ToHashSet(),
                Languages = gameProvider.Languages.Select(lang =>
                    new GameProviderLanguage
                    {
                        GameProviderId = gameProvider.Id,
                        CultureCode = lang.Code,
                        GameProviderCultureCode = lang.ExternalCode
                    }).ToHashSet()
            };

            gp.GameProviderConfigurations = gameProvider.GameProviderConfigurations.Select((gpc, idx) => new GameProviderConfiguration
            {
                Id = gpc.Id,
                Name = gpc.Name,
                ClientId = gpc.ClientId,
                SecretKey = gpc.SecretKey,
                DesktopEndpoint = gpc.DesktopEndpoint,
                MobileEndpoint = gpc.MobileEndpoint,
                Description = gp.Name,
                GameProviderId = gp.Id,
                CreatedOn = _now,
                UpdatedOn = _now,
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
                CreatedOn = _now,
                UpdatedOn = _now,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername
            }).ToHashSet();

            return gp;
        }

        private Reseller CreateResellerWithLicensee(IReseller r)
        {
            return new Reseller
            {
                Id = r.Id,
                Code = r.Code,
                Name = r.Name,
                OperationalEmail = r.OperationalEmail,
                Description = string.Empty,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername,
                CreatedOn = _now,
                UpdatedOn = _now,                
                Currencies = r.Currencies.Select(curr => new ResellerCurrency
                {
                    ResellerId = r.Id,
                    CurrencyCode = curr.Code
                }).ToHashSet(),
                Languages = r.Languages.Select(lang => new ResellerLanguage
                {
                    ResellerId = r.Id,
                    CultureCode = lang.Code
                }).ToHashSet(),
                Licensees = r.Licensees.Select(l => CreateLicensee(r, l)).ToHashSet(),
            };
        }

        private Licensee CreateLicensee(IReseller reseller, ILicensee licensee)
        {
            var gameProviders = licensee.GameProviders.Select(code => _gameProvidersCreated[code]);

            return new Licensee
            {
                Id = licensee.Id,
                Code = licensee.Code,
                Name = licensee.Name,
                Status = Status.Active,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername,
                CreatedOn = _now,
                UpdatedOn = _now,
                ResellerId = reseller.Id,
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
                    LicenseeLanguageCode = lang.ExternalCode
                }).ToHashSet(),
                GameProviders = gameProviders.Select(gp => new LicenseeGameProvider
                {
                    GameProviderId = gp.Id,
                    LicenseeId = reseller.Id
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

        private static void LogSeedingTime(Action getFunc,
            StringBuilder builder)
        {
            var timer = Stopwatch.StartNew();
            getFunc();
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