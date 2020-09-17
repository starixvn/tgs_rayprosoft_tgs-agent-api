using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Extensions;
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
        private const string AdminUsername = "admin.enabled";
        private readonly DateTimeOffset _now = DateTimeOffset.Now;
        private readonly Guid _resellerId = new Guid("5C0EFEFC-CCD4-423C-8CBF-6174CE08B1D3");
        private readonly Guid _licenseeId = new Guid("5C0EFEFC-CCD4-423C-8CBF-6174CE08B1D4");
        private ITgsRepository _repository;

        public DataInitializer(IServiceScopeFactory scopeFactory)
        {
            var serviceScope = scopeFactory.CreateScope();
            _repository = serviceScope.ServiceProvider.GetRequiredService<ITgsRepository>();
        }


        void IDataInitializer.SetInitialData()
        {
            if (_repository.Players.Any())
            {
                return;
            }

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

            GenerateCurrencies();
            GenerateLanguages();
            GenerateBetLimitGroups();
            GenerateVipLevel();
            GeneratePlayers();
        }

        private void GenerateCurrencies()
        {
            var _tmpCurrencies = new[]
            {
                new _Currency("CAD", "Canadian Dollar", "Canadian Dollar", "$"),
                new _Currency("CNY","Yuan", "元","¥"),
                new _Currency("EUR", "Euro", "Euro","€"),
                new _Currency("GBP", "Pound Sterling","Pound Sterling","£"),
                new _Currency("IDR", "Indonesian rupiah","rupiah Indonesia","Rp"),
                new _Currency("IDR_1000", "Indonesian rupiah","rupiah Indonesia","Rp"),
                new _Currency("INR", "Indian Rupee","भारतीय रुपये","₹"),
                new _Currency("JPY", "Japanese Yen","円","¥"),
                new _Currency("KRW", "Korean won","Korean won","₩"),
                new _Currency("MYR", "Malaysian Ringgit","Malaysian Ringgit","RM"),
                new _Currency("PLN", "Polish zloty","Polski złoty","zł"),
                new _Currency("RMB", "Yuan", "元","¥"),
                new _Currency("RUB", "Russian ruble","Российский рубль","₽"),
                new _Currency("SGD", "Singapore Dollar","Singapore Dollar","$"),
                new _Currency("THB", "Thai Baht","บาท","฿"),
                new _Currency("USD", "United States Dollar", "United States Dollar","$"),
                new _Currency("VND", "Vietnamese Dong","đồng","₫"),
                new _Currency("VND_1000", "Vietnamese Dong","đồng","₫")
            };

            var currencies = _tmpCurrencies.Select(p => new Currency
            {
                Code = p.Code,
                Name = p.Name,
                NativeName = p.NativeName,
                Symbol = p.Symbol,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername,
                CreatedOn = _now,
                UpdatedOn = _now
            }).ToArray();

            _repository.BulkInsert(currencies.ToArray());
        }

        readonly VipLevel[] _vipLevels = new[]
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

            _repository.BulkInsert(_vipLevels.ToArray());
        }

        private void GenerateLanguages()
        {
            var tmpLanguages = new[]
            {
                new _Language("en-US","en","English","English"),
                new _Language("id-ID","id","Indonesian","bahasa Indonesia"),
                new _Language("ja-JP","ja","Japanese","日本語"),
                new _Language("ko-KR","ko","Korean","한국어 / 조선말"),
                new _Language("th-TH","th","Thai","ภาษาไทย"),
                new _Language("vi-VN","vi","Vietnamese","ngôn ngữ tiếng Việt"),
                new _Language("zh-CN","zh","Simplified Chinese", "简体中文"),
                new _Language("zh-TW","zh","Traditional Chinese"," 繁體中文"),
            };

            var languages = tmpLanguages.Select(p => new Language
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
            _repository.BulkInsert(languages.ToArray());
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

            var tmpPlayers = new List<string>();
            var players = Enumerable.Range(1, 200)
                .Select(p =>
                {
                    var defaultWalletId = GuidExtensions.NewSequentialGuid();
                    var currencyCode = "RMB";
                    var playerCode = Generator.RandomString(8);
                    tmpPlayers.Add(playerCode);
                    return new Player
                    {
                        PlayerCode = playerCode,
                        Username = Generator.RandomString(12),
                        RealName = Generator.RandomString(12),
                        ExternalId = playerCode + "_" + Generator.RandomString(4),
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
    }

    public class Generator
    {
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string RandomIntString(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}