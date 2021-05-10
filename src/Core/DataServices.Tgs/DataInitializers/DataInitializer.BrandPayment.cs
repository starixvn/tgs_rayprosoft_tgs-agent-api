using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    public partial class DataInitializer
    {
        private static readonly Dictionary<string, Guid> PaymentChannels = new()
        {
            ["RichBank"] = new("E761A2D3-5705-417F-8D97-43CB7AEA0142"),
            ["USDT"] = new("B90029F9-F0C6-4E77-9031-0C554028F9BA")
        };

        private static readonly Dictionary<string, (Guid Id, string BankType)> PaymentMethods = new()
        {
            ["BankTransfer"] = (new Guid("67252F3A-6826-45CE-8BBC-C53E2CA7CDCB"), "Bank"),
            ["Crypto"] = (new Guid("798D21C6-9CF5-4FC0-BD09-8D67D2C9E7E1"), "Crypto")
        };

        private static readonly Dictionary<string, (Guid Id, string ExtendedData)> Payments = new()
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
                    }))
        };

        private async Task GenerateBrandPaymentsAsync()
        {
            var allPaymentMethodChannels = await _repository.PaymentMethodChannels.ToArrayAsync();
            var allBrands = Resellers
                .SelectMany(x => x.Licensees.SelectMany(l => l.Brands))
                .Distinct();

            var depositBrandPayments = from brand in allBrands
                                       from pmc in allPaymentMethodChannels
                                       let brandPaymentId = GuidExtensions.NewSequentialGuid()
                                       let payment = Payments.Where(p => p.Value.Id == pmc.PaymentId).First()
                                       select new BrandPayment
                                       {
                                           Id = brandPaymentId,
                                           DisplayOrder = 1,
                                           BrandId = brand.Id,
                                           PaymentMethodChannelId = pmc.Id,
                                           TransferType = PaymentTransferType.Deposit,
                                           ChannelName = $"{Payments.Where(p => p.Value.Id == pmc.PaymentId).First().Key}_" +
                                               $"{PaymentMethods.Where(p => p.Value.Id == pmc.MethodId).First().Key}_" +
                                               $"{PaymentChannels.Where(p => p.Value == pmc.ChannelId).First().Key}",
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
                                           BrandPaymentPlayers =
                                           new[]
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
                                           ExtendedData = payment.Value.ExtendedData,
                                           CreatedByUsername = AdminUsername,
                                           CreatedOn = DateTimeOffset.Now,
                                           UpdatedByUsername = AdminUsername,
                                           UpdatedOn = DateTimeOffset.Now
                                       };

            var withdrawBrandPayments = from brand in allBrands
                                        from pmc in allPaymentMethodChannels
                                        where pmc.PaymentId != Payments["Manual"].Id
                                        let brandPaymentId = GuidExtensions.NewSequentialGuid()
                                        let payment = Payments.Where(p => p.Value.Id == pmc.PaymentId).First()
                                        select new BrandPayment
                                        {
                                            Id = brandPaymentId,
                                            DisplayOrder = 1,
                                            BrandId = brand.Id,
                                            PaymentMethodChannelId = pmc.Id,
                                            TransferType = PaymentTransferType.Withdrawal,
                                            ChannelName = $"{Payments.Where(p => p.Value.Id == pmc.PaymentId).First().Key}_" +
                                                $"{PaymentMethods.Where(p => p.Value.Id == pmc.MethodId).First().Key}_" +
                                                $"{PaymentChannels.Where(p => p.Value == pmc.ChannelId).First().Key}",
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
                                            BrandPaymentPlayers =
                                            new[]
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
                                            ExtendedData = payment.Value.ExtendedData,
                                            CreatedByUsername = AdminUsername,
                                            CreatedOn = DateTimeOffset.Now,
                                            UpdatedByUsername = AdminUsername,
                                            UpdatedOn = DateTimeOffset.Now
                                        };

            await _repository.BrandPayments.AddRangeAsync(depositBrandPayments);
            await _repository.BrandPayments.AddRangeAsync(withdrawBrandPayments);
        }

        private async Task GeneratePaymentAsync()
        {
            await GeneratePaymentsAsync();
            await GeneratePaymentMethodsAsync();
            await GeneratePaymentChannelsAsync();
            await GeneratePaymentMethodChannelsAsync();

            await _repository.SaveChangesAsync();
            await GenerateBrandPaymentsAsync();
        }

        private Task GeneratePaymentChannelsAsync()
        {
            var paymentVendors = PaymentChannels.Select(p => new PaymentChannel
            {
                Id = p.Value,
                Code = p.Key,
                Description = "online payment channel",
            }).ToArray();

            return _repository.PaymentChannels.AddRangeAsync(paymentVendors);
        }

        private Task GeneratePaymentMethodChannelsAsync()
        {
            var paymentMethodChannels = new[] {
                new PaymentMethodChannel
                {
                    Id = Guid.NewGuid(),
                    PaymentId = Payments["Manual"].Id,
                    MethodId = PaymentMethods["BankTransfer"].Id,
                    ChannelId = PaymentChannels["RichBank"],
                    CreatedByUsername = AdminUsername,
                    CreatedOn = DateTimeOffset.Now,
                    UpdatedByUsername = AdminUsername,
                    UpdatedOn = DateTimeOffset.Now
                },
                new PaymentMethodChannel
                {
                    Id = Guid.NewGuid(),
                    PaymentId = Payments["LeePay"].Id,
                    MethodId = PaymentMethods["Crypto"].Id,
                    ChannelId = PaymentChannels["USDT"],
                    CreatedByUsername = AdminUsername,
                    CreatedOn = DateTimeOffset.Now,
                    UpdatedByUsername = AdminUsername,
                    UpdatedOn = DateTimeOffset.Now
                }
            };
            var query = from payment in Payments
                        from method in PaymentMethods
                        let ChannelId = method.Value.BankType switch
                        {
                            "Bank" => PaymentChannels["RichBank"],
                            "Crypto" => PaymentChannels["USDT"],
                            _ => PaymentChannels["USDT"]
                        }
                        select new PaymentMethodChannel
                        {
                            Id = Guid.NewGuid(),
                            PaymentId = payment.Value.Id,
                            MethodId = method.Value.Id,
                            ChannelId = ChannelId,
                            CreatedByUsername = AdminUsername,
                            CreatedOn = DateTimeOffset.Now,
                            UpdatedByUsername = AdminUsername,
                            UpdatedOn = DateTimeOffset.Now
                        };

            return _repository.PaymentMethodChannels.AddRangeAsync(query);
        }

        private Task GeneratePaymentMethodsAsync()
        {
            var paymentMethods = PaymentMethods.Select(p => new PaymentMethod
            {
                Id = p.Value.Id,
                Code = p.Key,
                BankType = p.Value.BankType,
                Description = "online payment method",
            }).ToArray();

            return _repository.PaymentMethods.AddRangeAsync(paymentMethods);
        }

        private Task GeneratePaymentsAsync()
        {
            var payments = Payments.Select(p => new Payment
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

            return _repository.Payments.AddRangeAsync(payments);
        }
    }
}
