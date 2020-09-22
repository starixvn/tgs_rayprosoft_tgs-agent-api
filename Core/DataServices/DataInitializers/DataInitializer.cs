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
        private readonly IServiceLog<DataInitializer> _log;
        private readonly DateTimeOffset _now = DateTimeOffset.Now;
        private readonly Guid _resellerId = new Guid("5C0EFEFC-CCD4-423C-8CBF-6174CE08B1D3");
        private readonly Guid _licenseeId = new Guid("5C0EFEFC-CCD4-423C-8CBF-6174CE08B1D4");

        private readonly StringBuilder _logBuilder = new StringBuilder();

        private static readonly object Locker = new object();
        private static readonly string AdminUsername = "admin.enabled";

        public DataInitializer(ITgsRepository repository, IServiceLog<DataInitializer> log)
        {
            _repository = repository;
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

            LogSeedingTime(GenerateResellersAndLicensees, _logBuilder);
            LogSeedingTime(GenerateCurrencies, _logBuilder);
            LogSeedingTime(GenerateLanguages, _logBuilder);
            LogSeedingTime(GenerateBetLimitGroups, _logBuilder);
            LogSeedingTime(GenerateVipLevel, _logBuilder);
            LogSeedingTime(GeneratePlayers, _logBuilder);

            timer.Stop();

            _logBuilder.AppendLine($"Total Time was {timer.ElapsedMilliseconds} ms");
            _log.LogInformation(_logBuilder.ToString());
        }

        private void GenerateResellersAndLicensees()
        {
            var reseller = new Reseller
            {
                Id = _resellerId,
                Code = "TGS",
                Name = "Turnkey Game System",
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername,
                CreatedOn = _now,
                UpdatedOn = _now
            };

            var licensee = new Licensee
            {
                Id = _licenseeId,
                ResellerId = _resellerId,
                Code = "TGS",
                Name = "Turnkey Game System",
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername,
                CreatedOn = _now,
                UpdatedOn = _now
            };

            _repository.Resellers.Add(reseller);
            _repository.Licensees.Add(licensee);
            _repository.SaveChanges();
        }

        private void GenerateCurrencies()
        {
            var currencies = new[]
            {
                new { Code = "CAD", Name = "Canadian Dollar", NativeName = "Canadian Dollar", Symbol = "$" },
                new { Code = "CNY", Name = "Yuan", NativeName = "元", Symbol ="¥" },
                new { Code = "EUR", Name = "Euro", NativeName = "Euro", Symbol ="€" },
                new { Code = "GBP", Name = "Pound Sterling", NativeName = "Pound Sterling", Symbol ="£" },
                new { Code = "IDR", Name = "Indonesian rupiah", NativeName = "rupiah Indonesia", Symbol ="Rp" },
                new { Code = "IDR_1000", Name = "Indonesian rupiah", NativeName = "rupiah Indonesia", Symbol ="Rp" },
                new { Code = "INR", Name = "Indian Rupee", NativeName = "भारतीय रुपये", Symbol ="₹" },
                new { Code = "JPY", Name = "Japanese Yen", NativeName = "円", Symbol ="¥" },
                new { Code = "KRW", Name = "Korean won", NativeName = "Korean won", Symbol ="₩" },
                new { Code = "MYR", Name = "Malaysian Ringgit", NativeName = "Malaysian Ringgit", Symbol ="RM" },
                new { Code = "PLN", Name = "Polish zloty", NativeName = "Polski złoty", Symbol ="zł" },
                new { Code = "RUB", Name = "Russian ruble", NativeName = "Российский рубль", Symbol ="₽" },
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
            new VipLevel{ Id = GuidExtensions.NewSequentialGuid(), Code = "VIP 0", Name = "名称A" },
            new VipLevel{ Id = GuidExtensions.NewSequentialGuid(), Code = "VIP 1", Name = "名称B" },
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
                vip.CreatedOn = DateTimeOffset.Now;
                vip.UpdatedOn = DateTimeOffset.Now;
            }
            _repository.BulkInsert(_vipLevels.ToArray());
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
                CreatedOn = DateTimeOffset.Now,
                UpdatedByUsername = AdminUsername,
                UpdatedOn = DateTimeOffset.Now
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
            _repository.BulkInsert(betLimitGroups.ToArray());
        }

        private void GeneratePlayers()
        {
            var rnd = new Random();
            var (password, salt) = GenerateCredential("TgsP@ssw0rd");
            var tmpPlayers = new List<string>();
            var players = Enumerable.Range(1, 200)
                .Select(p =>
                {
                    var defaultWalletId = GuidExtensions.NewSequentialGuid();
                    var currencyCode = "CNY";
                    var playerCode = Generator.RandomString(12);
                    tmpPlayers.Add(playerCode);
                    return new Player
                    {
                        PlayerCode = playerCode,
                        Username = Generator.RandomString(12),
                        RealName = Generator.RandomString(12),
                        ExternalId = $"{playerCode}_{Generator.RandomString(4)}",
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
                        ParentPlayerCode = tmpPlayers.Count == 1 ? null : rnd.NextDouble() > 0.5 ? null : tmpPlayers[rnd.Next(0, tmpPlayers.Count - 1)],
                        HasAgent = rnd.NextDouble() > 0.5,
                        VipId = _vipLevels[rnd.Next(0, 6)].Id,
                        RegisteredOn = _now,
                        LastActiveOn = _now,
                        LicenseeId = _licenseeId,
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

            _repository.BulkInsert(players.ToArray());
            _repository.BulkInsert(players.SelectMany(p => p.Wallets).ToArray());
            _repository.BulkInsert(players.SelectMany(p => p.Banks).ToArray());
        }

        private (byte[], byte[]) GenerateCredential(string password)
        {
            ITgsHashConfig tgsHashConfig = new TgsHashConfig();
            IHashProvider hashProvider = new HashProvider(tgsHashConfig);

            var salt = hashProvider.GenerateRandomSalt(32);
            var hashedPassword = hashProvider.Hash(
                Encoding.UTF8.GetBytes(password),
                hashProvider.Hash(salt),
                tgsHashConfig.HashAlgorithm);
            return (salt, hashedPassword);
        }

        private void LogSeedingTime(Action getFunc,
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