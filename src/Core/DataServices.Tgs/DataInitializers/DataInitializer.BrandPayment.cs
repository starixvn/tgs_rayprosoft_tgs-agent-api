using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    public partial class DataInitializer
    {
        private static readonly Dictionary<string, Guid> _PaymentChannels = new()
        {
            ["上海浦东发展银行"] = new Guid("E761A2D3-5705-417F-8D97-43CB7AEA0142"),
            ["中信银行"] = new Guid("DC642A94-C2A3-4E89-A71D-891E69D2E3CA"),
            ["中国光大银行"] = new Guid("9BA8107A-F638-42DF-9550-97E6CF39635B"),
            ["中国农业银行"] = new Guid("AFEDFEE0-869B-45C9-81C0-53B9EE893F87"),
            ["中国工商银行"] = new Guid("993A41B8-A090-48E8-97F0-2DDA0E5921F9"),
            ["中国建设银行"] = new Guid("AE24A9EF-B18C-4CA1-B0B9-3540A70A0A55"),
            ["中国民生银行"] = new Guid("ADB50CC2-F96F-4892-A555-93F2564AF231"),
            ["中国银行"] = new Guid("76FD5896-B659-4D73-BFB8-D5EAF6AE71C3"),
            ["交通银行"] = new Guid("8B7320C8-BC4F-46D6-A207-84490DA9AB40"),
            ["兴业银行"] = new Guid("9E7492BC-5D42-43E5-B202-8AA1F841AB85"),
            ["农村商业银行"] = new Guid("60B75917-13E7-481C-BEE2-3F772E640AFC"),
            ["华夏银行"] = new Guid("02F1C110-74B9-4AC8-82DE-808CBB63AAA6"),
            ["城市商业银行"] = new Guid("133B2C82-15F4-4D49-9CE1-7B9016080579"),
            ["平安银行"] = new Guid("8F311F11-81B3-4E15-980B-2B215B15299E"),
            ["广发银行"] = new Guid("F69BDC10-2672-4BAE-B56C-74312238E17A"),
            ["恒丰银行"] = new Guid("5DA03236-2CBC-4943-996A-243F22C725E3"),
            ["招商银行"] = new Guid("32F5823C-556E-42E9-A1DB-4B2F37F147EC"),
            ["浙江农信"] = new Guid("C71A8A66-C256-4387-8EFC-22FBC3BBBC3E"),
            ["渤海银行"] = new Guid("39F2EEDD-8D53-4276-8840-58F48282369E"),
            ["福建省农村信用社"] = new Guid("AFC7830D-2949-4A75-B63E-8206DA684FA0"),
            ["邮政储汇"] = new Guid("AA1A7612-97C3-4CA5-985E-BADC269C4E17"),
            ["鄞州银行"] = new Guid("EA89F52C-61A7-4727-B1E3-DC08F280C6C6"),
            ["ETHEREUM"] = new Guid("B90029F9-F0C6-4E77-9031-0C554028F9BA"),
            ["TRON"] = new Guid("9D76809A-4611-457D-8F49-D341D3A464EA")
        };

        private static readonly Dictionary<string, (Guid Id, string BankType)> _PaymentMethods = new()
        {
            ["BankTransfer"] = (new Guid("67252F3A-6826-45CE-8BBC-C53E2CA7CDCB"), "Bank"),
            ["Crypto"] = (new Guid("798D21C6-9CF5-4FC0-BD09-8D67D2C9E7E1"), "Crypto")
        };

        private static readonly Dictionary<string, (Guid Id, string ExtendedData)> _Payments = new()
        {
            ["Manual"] = (
                new Guid("30CB4D17-2506-4D72-8470-65063B80F9C9"),
                JsonSerializer.Serialize(
                    new
                    {
                        BankAccount = "9876543210",
                        Name = "測試者"
                    },
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = false
                    })),
            ["LeePay"] = (
                new Guid("EC6EBD49-E002-4DB9-B7DE-1750FADC0C8E"),
                JsonSerializer.Serialize(
                    new
                    {
                        SystemId = "WG",
                        SystemKey = "7ujm5tgB",
                        ChannelCurrency = "CNY",
                        Channel = "TRON"
                    },
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = false
                    })),
            ["PrimePay"] = (
                new Guid("E7D2533B-7319-4113-B1A1-7072CB6BB8D3"),
                JsonSerializer.Serialize(
                    new
                    {
                        MerchantId = "c3a77b3187759940",
                        Key = "018a716fafde1b7f",
                        IV = "2c956cdbfabc792a",
                        Currency = "RMB"
                    }))
        };

        private static readonly Guid _CustomChannelId = new Guid("3E3CC1EB-1A4E-413F-926C-1AD48707BC66");

        private async Task GenerateBrandPaymentsAsync()
        {
            await GenerateDepositBrandPaymentsAsync();
            await GenerateWithdrawBrandPaymentsAsync();
        }

        private async Task GenerateDepositBrandPaymentsAsync()
        {
            var allPaymentMethodChannels = await _repository.PaymentMethodChannels.ToArrayAsync();
            var allBrands = Resellers
                .SelectMany(x => x.Licensees.SelectMany(l => l.Brands))
                .Distinct();

            var manualDepositBrandPayments = from brand in allBrands
                                             from pmc in allPaymentMethodChannels
                                             where pmc.PaymentId == _Payments["Manual"].Id
                                                && pmc.ChannelId != _CustomChannelId
                                             let brandPaymentId = GuidExtensions.NewSequentialGuid()
                                             select new BrandPayment
                                             {
                                                 Id = brandPaymentId,
                                                 DisplayOrder = 1,
                                                 BrandId = brand.Id,
                                                 PaymentMethodChannelId = pmc.Id,
                                                 TransferType = PaymentTransferType.Deposit,
                                                 ChannelName = $"{_Payments.Where(p => p.Value.Id == pmc.PaymentId).First().Key}_" +
                                                     $"{_PaymentMethods.Where(p => p.Value.Id == pmc.MethodId).First().Key}_" +
                                                     $"{_PaymentChannels.Where(p => p.Value == pmc.ChannelId).FirstOrDefault().Key ?? "Custom" }",
                                                 PaymentStatus = SwitchStatus.Enabled,
                                                 TransferLimitFrom = 10,
                                                 TransferLimitTo = 100000,
                                                 PlayerListCondition = PlayerListCondition.MatchAll,
                                                 AllPlayers = true,
                                                 IdentityAuthentication = false,
                                                 PhoneAuthentication = false,
                                                 DepositedAmountUnlimited = true,
                                                 DepositedAmountMin = 0M,
                                                 DepositedAmountMax = 0M,
                                                 Reason = "Reason",
                                                 FeePercentage = 50,
                                                 LowestFee = 50,
                                                 HighestFee = 500,
                                                 BrandPaymentPlayers = new[]
                                                 {
                                                     new BrandPaymentPlayer
                                                     {
                                                         PlayerCode = _players[0].PlayerCode,
                                                         ListType = PlayerListType.White,
                                                         BrandPaymentId = brandPaymentId
                                                     },
                                                     new BrandPaymentPlayer
                                                     {
                                                         PlayerCode = _players[1].PlayerCode,
                                                         ListType = PlayerListType.Black,
                                                         BrandPaymentId = brandPaymentId
                                                     }
                                                 },
                                                 VipLevels = new[]
                                                 {
                                                     new BrandPaymentVipLevel { BrandPaymentId = brandPaymentId, VipLevelId = _vipLevels[0].Id },
                                                     new BrandPaymentVipLevel { BrandPaymentId = brandPaymentId, VipLevelId = _vipLevels[1].Id }
                                                 },
                                                 ExtendedData = JsonSerializer.Serialize(
                                                     new
                                                     {
                                                         BankAccount = "9876543210",
                                                         Name = "測試者"
                                                     },
                                                     new JsonSerializerOptions
                                                     {
                                                         PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                                         WriteIndented = false
                                                     }),
                                                 CreatedByUsername = AdminUsername,
                                                 CreatedOn = DateTimeOffset.Now,
                                                 UpdatedByUsername = AdminUsername,
                                                 UpdatedOn = DateTimeOffset.Now
                                             };

            var leepayBrandPayments = from brand in allBrands
                                      from pmc in allPaymentMethodChannels
                                      where pmc.PaymentId == _Payments["LeePay"].Id
                                      let brandPaymentId = GuidExtensions.NewSequentialGuid()
                                      select new BrandPayment
                                      {
                                          Id = brandPaymentId,
                                          DisplayOrder = 1,
                                          BrandId = brand.Id,
                                          PaymentMethodChannelId = pmc.Id,
                                          TransferType = PaymentTransferType.Deposit,
                                          ChannelName = $"{_Payments.Where(p => p.Value.Id == pmc.PaymentId).First().Key}_" +
                                              $"{_PaymentMethods.Where(p => p.Value.Id == pmc.MethodId).First().Key}_" +
                                              $"{_PaymentChannels.Where(p => p.Value == pmc.ChannelId).First().Key}",
                                          PaymentStatus = SwitchStatus.Enabled,
                                          TransferLimitFrom = 10,
                                          TransferLimitTo = 100000,
                                          PlayerListCondition = PlayerListCondition.MatchAll,
                                          AllPlayers = true,
                                          IdentityAuthentication = false,
                                          PhoneAuthentication = false,
                                          DepositedAmountUnlimited = true,
                                          DepositedAmountMin = 0M,
                                          DepositedAmountMax = 0M,
                                          Reason = "Reason",
                                          FeePercentage = 50,
                                          LowestFee = 50,
                                          HighestFee = 500,
                                          BrandPaymentPlayers = new[]
                                          {
                                              new BrandPaymentPlayer
                                              {
                                                  PlayerCode = _players[0].PlayerCode,
                                                  ListType = PlayerListType.White,
                                                  BrandPaymentId = brandPaymentId
                                              },
                                              new BrandPaymentPlayer
                                              {
                                                  PlayerCode = _players[1].PlayerCode,
                                                  ListType = PlayerListType.Black,
                                                  BrandPaymentId = brandPaymentId
                                              }
                                          },
                                          VipLevels = new[]
                                          {
                                              new BrandPaymentVipLevel { BrandPaymentId = brandPaymentId, VipLevelId = _vipLevels[0].Id },
                                              new BrandPaymentVipLevel { BrandPaymentId = brandPaymentId, VipLevelId = _vipLevels[1].Id }
                                          },
                                          ExtendedData = JsonSerializer.Serialize(
                                              new
                                              {
                                                  SystemId = "WG",
                                                  SystemKey = "7ujm5tgB",
                                                  ChannelCurrency = "CNY",
                                                  Channel = "TRON"
                                              },
                                              new JsonSerializerOptions
                                              {
                                                  PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                                  WriteIndented = false
                                              }),
                                          CreatedByUsername = AdminUsername,
                                          CreatedOn = DateTimeOffset.Now,
                                          UpdatedByUsername = AdminUsername,
                                          UpdatedOn = DateTimeOffset.Now
                                      };

            var primepayBrandPayments = from brand in allBrands
                                        from pmc in allPaymentMethodChannels
                                        where pmc.PaymentId == _Payments["PrimePay"].Id
                                            && pmc.ChannelId == _CustomChannelId
                                        let brandPaymentId = GuidExtensions.NewSequentialGuid()
                                        select new BrandPayment
                                        {
                                            Id = brandPaymentId,
                                            DisplayOrder = 1,
                                            BrandId = brand.Id,
                                            PaymentMethodChannelId = pmc.Id,
                                            TransferType = PaymentTransferType.Deposit,
                                            ChannelName = $"{_Payments.Where(p => p.Value.Id == pmc.PaymentId).First().Key}_" +
                                                $"{_PaymentMethods.Where(p => p.Value.Id == pmc.MethodId).First().Key}_" +
                                                $"{_PaymentChannels.Where(p => p.Value == pmc.ChannelId).FirstOrDefault().Key ?? "Custom"}",
                                            PaymentStatus = SwitchStatus.Enabled,
                                            TransferLimitFrom = 10,
                                            TransferLimitTo = 100000,
                                            PlayerListCondition = PlayerListCondition.MatchAll,
                                            AllPlayers = true,
                                            IdentityAuthentication = false,
                                            PhoneAuthentication = false,
                                            DepositedAmountUnlimited = true,
                                            DepositedAmountMin = 0M,
                                            DepositedAmountMax = 0M,
                                            Reason = "Reason",
                                            FeePercentage = 50,
                                            LowestFee = 50,
                                            HighestFee = 500,
                                            BrandPaymentPlayers = new[]
                                            {
                                                new BrandPaymentPlayer
                                                {
                                                    PlayerCode = _players[0].PlayerCode,
                                                    ListType = PlayerListType.White,
                                                    BrandPaymentId = brandPaymentId
                                                },
                                                new BrandPaymentPlayer
                                                {
                                                    PlayerCode = _players[1].PlayerCode,
                                                    ListType = PlayerListType.Black,
                                                    BrandPaymentId = brandPaymentId
                                                }
                                            },
                                            VipLevels = new[]
                                            {
                                                new BrandPaymentVipLevel { BrandPaymentId = brandPaymentId, VipLevelId = _vipLevels[0].Id },
                                                new BrandPaymentVipLevel { BrandPaymentId = brandPaymentId, VipLevelId = _vipLevels[1].Id }
                                            },
                                            ExtendedData = JsonSerializer.Serialize(
                                                new
                                                {
                                                    MerchantId = "c3a77b3187759940",
                                                    Key = "018a716fafde1b7f",
                                                    IV = "2c956cdbfabc792a",
                                                    Currency = "RMB"
                                                },
                                                new JsonSerializerOptions
                                                {
                                                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                                    WriteIndented = false
                                                }),
                                            CreatedByUsername = AdminUsername,
                                            CreatedOn = DateTimeOffset.Now,
                                            UpdatedByUsername = AdminUsername,
                                            UpdatedOn = DateTimeOffset.Now
                                        };

            await _repository.BrandPayments.AddRangeAsync(
                manualDepositBrandPayments.Concat(leepayBrandPayments).Concat(primepayBrandPayments));

            await _repository.SaveChangesAsync();
        }

        private async Task GeneratePaymentAsync()
        {
            await GeneratePaymentsAsync();
            await GeneratePaymentMethodsAsync();
            await GeneratePaymentChannelsAsync();
            await GeneratePaymentMethodChannelsAsync();
            await GenerateBrandPaymentsAsync();
        }

        private async Task GeneratePaymentChannelsAsync()
        {
            var paymentVendors = _PaymentChannels.Select(p => new PaymentChannel
            {
                Id = p.Value,
                Code = p.Key,
                Description = "online payment channel",
            }).ToArray();

            await _repository.PaymentChannels.AddRangeAsync(paymentVendors);
            await _repository.PaymentChannels.AddAsync(new PaymentChannel
            {
                Id = _CustomChannelId,
                Code = "CustomBank",
                Description = "online payment channel"
            });

            await _repository.SaveChangesAsync();
        }

        private async Task GeneratePaymentMethodChannelsAsync()
        {
            var manualList = from channel in _PaymentChannels
                             where !(new[] { "ETHEREUM", "TRON" }.Contains(channel.Key))
                             select new PaymentMethodChannel
                             {
                                 Id = GuidExtensions.NewSequentialGuid(),
                                 PaymentId = _Payments["Manual"].Id,
                                 MethodId = _PaymentMethods["BankTransfer"].Id,
                                 ChannelId = channel.Value,
                                 CreatedByUsername = AdminUsername,
                                 CreatedOn = DateTimeOffset.Now,
                                 UpdatedByUsername = AdminUsername,
                                 UpdatedOn = DateTimeOffset.Now
                             };

            var leepayList = new[] {
                new PaymentMethodChannel
                {
                    Id = GuidExtensions.NewSequentialGuid(),
                    PaymentId = _Payments["LeePay"].Id,
                    MethodId = _PaymentMethods["Crypto"].Id,
                    ChannelId = _PaymentChannels["ETHEREUM"],
                    CreatedByUsername = AdminUsername,
                    CreatedOn = DateTimeOffset.Now,
                    UpdatedByUsername = AdminUsername,
                    UpdatedOn = DateTimeOffset.Now
                },
                new PaymentMethodChannel
                {
                    Id = GuidExtensions.NewSequentialGuid(),
                    PaymentId = _Payments["LeePay"].Id,
                    MethodId = _PaymentMethods["Crypto"].Id,
                    ChannelId = _PaymentChannels["TRON"],
                    CreatedByUsername = AdminUsername,
                    CreatedOn = DateTimeOffset.Now,
                    UpdatedByUsername = AdminUsername,
                    UpdatedOn = DateTimeOffset.Now
                },
            };

            var primepayList = from channel in _PaymentChannels
                               where !(new[] { "ETHEREUM", "TRON" }.Contains(channel.Key))
                               select new PaymentMethodChannel
                               {
                                   Id = GuidExtensions.NewSequentialGuid(),
                                   PaymentId = _Payments["PrimePay"].Id,
                                   MethodId = _PaymentMethods["BankTransfer"].Id,
                                   ChannelId = channel.Value,
                                   CreatedByUsername = AdminUsername,
                                   CreatedOn = DateTimeOffset.Now,
                                   UpdatedByUsername = AdminUsername,
                                   UpdatedOn = DateTimeOffset.Now
                               };

            await _repository.PaymentMethodChannels.AddRangeAsync(manualList.Concat(leepayList).Concat(primepayList));
            await _repository.PaymentMethodChannels.AddAsync(new PaymentMethodChannel
            {
                Id = GuidExtensions.NewSequentialGuid(),
                PaymentId = _Payments["Manual"].Id,
                MethodId = _PaymentMethods["BankTransfer"].Id,
                ChannelId = _CustomChannelId,
                CreatedByUsername = AdminUsername,
                CreatedOn = DateTimeOffset.Now,
                UpdatedByUsername = AdminUsername,
                UpdatedOn = DateTimeOffset.Now
            });
            await _repository.PaymentMethodChannels.AddAsync(new PaymentMethodChannel
            {
                Id = GuidExtensions.NewSequentialGuid(),
                PaymentId = _Payments["PrimePay"].Id,
                MethodId = _PaymentMethods["BankTransfer"].Id,
                ChannelId = _CustomChannelId,
                CreatedByUsername = AdminUsername,
                CreatedOn = DateTimeOffset.Now,
                UpdatedByUsername = AdminUsername,
                UpdatedOn = DateTimeOffset.Now
            });
            await _repository.SaveChangesAsync();
        }

        private async Task GeneratePaymentMethodsAsync()
        {
            var paymentMethods = _PaymentMethods.Select(p => new PaymentMethod
            {
                Id = p.Value.Id,
                Code = p.Key,
                BankType = p.Value.BankType,
                Description = "online payment method",
            }).ToArray();

            await _repository.PaymentMethods.AddRangeAsync(paymentMethods);
            await _repository.SaveChangesAsync();
        }

        private async Task GeneratePaymentsAsync()
        {
            var payments = _Payments.Select(p => new Payment
            {
                Id = p.Value.Id,
                Code = p.Key,
                Name = p.Key,
                Description = "online payment platform",
                CreatedByUsername = AdminUsername,
                CreatedOn = DateTimeOffset.Now,
                UpdatedByUsername = AdminUsername,
                UpdatedOn = DateTimeOffset.Now
            }).ToArray();

            await _repository.Payments.AddRangeAsync(payments);
            await _repository.SaveChangesAsync();
        }

        private async Task GenerateWithdrawBrandPaymentsAsync()
        {
            var allPaymentMethodChannels = await _repository.PaymentMethodChannels.ToArrayAsync();
            var allBrands = Resellers
                .SelectMany(x => x.Licensees.SelectMany(l => l.Brands))
                .Distinct();

            var manualBrandPayments = from brand in allBrands
                                      from pmc in allPaymentMethodChannels
                                      where pmc.PaymentId == _Payments["Manual"].Id
                                        && pmc.ChannelId == _CustomChannelId
                                      let brandPaymentId = GuidExtensions.NewSequentialGuid()
                                      select new BrandPayment
                                      {
                                          Id = brandPaymentId,
                                          DisplayOrder = 1,
                                          BrandId = brand.Id,
                                          PaymentMethodChannelId = pmc.Id,
                                          TransferType = PaymentTransferType.Withdrawal,
                                          ChannelName = $"{_Payments.Where(p => p.Value.Id == pmc.PaymentId).First().Key}_" +
                                              $"{_PaymentMethods.Where(p => p.Value.Id == pmc.MethodId).First().Key}_" +
                                              $"Custom",
                                          PaymentStatus = SwitchStatus.Enabled,
                                          TransferLimitFrom = 10,
                                          TransferLimitTo = 100000,
                                          PlayerListCondition = PlayerListCondition.MatchAll,
                                          AllPlayers = true,
                                          IdentityAuthentication = false,
                                          PhoneAuthentication = false,
                                          DepositedAmountUnlimited = true,
                                          DepositedAmountMin = 0M,
                                          DepositedAmountMax = 0M,
                                          Reason = "Reason",
                                          FeePercentage = 50,
                                          LowestFee = 50,
                                          HighestFee = 500,
                                          BrandPaymentPlayers = new[]
                                          {
                                              new BrandPaymentPlayer
                                              {
                                                  PlayerCode = _players[0].PlayerCode,
                                                  ListType = PlayerListType.White,
                                                  BrandPaymentId = brandPaymentId
                                              },
                                              new BrandPaymentPlayer
                                              {
                                                  PlayerCode = _players[1].PlayerCode,
                                                  ListType = PlayerListType.Black,
                                                  BrandPaymentId = brandPaymentId
                                              }
                                          },
                                          VipLevels = new[]
                                          {
                                              new BrandPaymentVipLevel { BrandPaymentId = brandPaymentId, VipLevelId = _vipLevels[0].Id },
                                              new BrandPaymentVipLevel { BrandPaymentId = brandPaymentId, VipLevelId = _vipLevels[1].Id }
                                          },
                                          ExtendedData = "{}",
                                          CreatedByUsername = AdminUsername,
                                          CreatedOn = DateTimeOffset.Now,
                                          UpdatedByUsername = AdminUsername,
                                          UpdatedOn = DateTimeOffset.Now
                                      };

            var leepayBrandPayments = from brand in allBrands
                                      from pmc in allPaymentMethodChannels
                                      where pmc.PaymentId == _Payments["LeePay"].Id
                                      let brandPaymentId = GuidExtensions.NewSequentialGuid()
                                      let channelType = pmc.ChannelId == _PaymentChannels["ETHEREUM"] ? "ETHEREUM" : "TRON"
                                      select new BrandPayment
                                      {
                                          Id = brandPaymentId,
                                          DisplayOrder = 1,
                                          BrandId = brand.Id,
                                          PaymentMethodChannelId = pmc.Id,
                                          TransferType = PaymentTransferType.Withdrawal,
                                          ChannelName = $"{_Payments.Where(p => p.Value.Id == pmc.PaymentId).First().Key}_" +
                                              $"{_PaymentMethods.Where(p => p.Value.Id == pmc.MethodId).First().Key}_" +
                                              $"{_PaymentChannels.Where(p => p.Value == pmc.ChannelId).First().Key}",
                                          PaymentStatus = SwitchStatus.Enabled,
                                          TransferLimitFrom = 10,
                                          TransferLimitTo = 100000,
                                          PlayerListCondition = PlayerListCondition.MatchAll,
                                          AllPlayers = true,
                                          IdentityAuthentication = false,
                                          PhoneAuthentication = false,
                                          DepositedAmountUnlimited = true,
                                          DepositedAmountMin = 0M,
                                          DepositedAmountMax = 0M,
                                          Reason = "Reason",
                                          FeePercentage = 50,
                                          LowestFee = 50,
                                          HighestFee = 500,
                                          BrandPaymentPlayers = new[]
                                          {
                                              new BrandPaymentPlayer
                                              {
                                                  PlayerCode = _players[0].PlayerCode,
                                                  ListType = PlayerListType.White,
                                                  BrandPaymentId = brandPaymentId
                                              },
                                              new BrandPaymentPlayer
                                              {
                                                  PlayerCode = _players[1].PlayerCode,
                                                  ListType = PlayerListType.Black,
                                                  BrandPaymentId = brandPaymentId
                                              }
                                          },
                                          VipLevels = new[]
                                          {
                                              new BrandPaymentVipLevel { BrandPaymentId = brandPaymentId, VipLevelId = _vipLevels[0].Id },
                                              new BrandPaymentVipLevel { BrandPaymentId = brandPaymentId, VipLevelId = _vipLevels[1].Id }
                                          },
                                          ExtendedData = JsonSerializer.Serialize(
                                              new
                                              {
                                                  SystemId = "WG",
                                                  SystemKey = "7ujm5tgB",
                                                  ChannelCurrency = "CNY",
                                                  Channel = channelType
                                              },
                                              new JsonSerializerOptions
                                              {
                                                  PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                                  WriteIndented = false
                                              }),
                                          CreatedByUsername = AdminUsername,
                                          CreatedOn = DateTimeOffset.Now,
                                          UpdatedByUsername = AdminUsername,
                                          UpdatedOn = DateTimeOffset.Now
                                      };

            var primepayBrandPayments = from brand in allBrands
                                        from pmc in allPaymentMethodChannels
                                        where pmc.PaymentId == _Payments["PrimePay"].Id
                                            && pmc.ChannelId == _CustomChannelId
                                        let brandPaymentId = GuidExtensions.NewSequentialGuid()
                                        select new BrandPayment
                                        {
                                            Id = brandPaymentId,
                                            DisplayOrder = 1,
                                            BrandId = brand.Id,
                                            PaymentMethodChannelId = pmc.Id,
                                            TransferType = PaymentTransferType.Withdrawal,
                                            ChannelName = $"{_Payments.Where(p => p.Value.Id == pmc.PaymentId).First().Key}_" +
                                                $"{_PaymentMethods.Where(p => p.Value.Id == pmc.MethodId).First().Key}_" +
                                                $"Custom",
                                            PaymentStatus = SwitchStatus.Enabled,
                                            TransferLimitFrom = 10,
                                            TransferLimitTo = 100000,
                                            PlayerListCondition = PlayerListCondition.MatchAll,
                                            AllPlayers = true,
                                            IdentityAuthentication = false,
                                            PhoneAuthentication = false,
                                            DepositedAmountUnlimited = true,
                                            DepositedAmountMin = 0M,
                                            DepositedAmountMax = 0M,
                                            Reason = "Reason",
                                            FeePercentage = 50,
                                            LowestFee = 50,
                                            HighestFee = 500,
                                            BrandPaymentPlayers = new[]
                                            {
                                                new BrandPaymentPlayer
                                                {
                                                    PlayerCode = _players[0].PlayerCode,
                                                    ListType = PlayerListType.White,
                                                    BrandPaymentId = brandPaymentId
                                                },
                                                new BrandPaymentPlayer
                                                {
                                                    PlayerCode = _players[1].PlayerCode,
                                                    ListType = PlayerListType.Black,
                                                    BrandPaymentId = brandPaymentId
                                                }
                                            },
                                            VipLevels = new[]
                                            {
                                                new BrandPaymentVipLevel { BrandPaymentId = brandPaymentId, VipLevelId = _vipLevels[0].Id },
                                                new BrandPaymentVipLevel { BrandPaymentId = brandPaymentId, VipLevelId = _vipLevels[1].Id }
                                            },
                                            ExtendedData = JsonSerializer.Serialize(
                                                new
                                                {
                                                    MerchantId = "c3a77b3187759940",
                                                    Key = "018a716fafde1b7f",
                                                    IV = "2c956cdbfabc792a",
                                                    Currency = "RMB"
                                                },
                                                new JsonSerializerOptions
                                                {
                                                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                                    WriteIndented = false
                                                }),
                                            CreatedByUsername = AdminUsername,
                                            CreatedOn = DateTimeOffset.Now,
                                            UpdatedByUsername = AdminUsername,
                                            UpdatedOn = DateTimeOffset.Now
                                        };

            await _repository.BrandPayments.AddRangeAsync(
                manualBrandPayments.Concat(leepayBrandPayments).Concat(primepayBrandPayments));
            await _repository.SaveChangesAsync();
        }
    }
}
