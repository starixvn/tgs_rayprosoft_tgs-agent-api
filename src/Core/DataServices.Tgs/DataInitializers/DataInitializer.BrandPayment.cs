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
        private const string Help2Pay = "Help2Pay";

        private const string AlipayPayment = "Alipay";
        private const string WeChatPayPayment = "WeChatPay";

        private const string AlipayScan = "AlipayScan";
        private const string WeChatPayScan = "WeChatPayScan";
        private const string BankTransfer = "BankTransfer";

        private static readonly Dictionary<string, Guid> Payments = new()
        {
            { Help2Pay, new("30CB4D17-2506-4D72-8470-65063B80F9C9") }
        };

        private static readonly Dictionary<string, (Guid, Guid)> PaymentMethods = new()
        {
            { AlipayPayment, (new("67252F3A-6826-45CE-8BBC-C53E2CA7CDCB"), Payments[Help2Pay]) },
            { WeChatPayPayment, (new("083D8754-1FE3-478A-9417-80DA796C3450"), Payments[Help2Pay]) }
        };

        private static readonly Dictionary<string, (Guid, (Guid, Guid))> PaymentChannels =
            new()
            {
                { AlipayScan, (new("E761A2D3-5705-417F-8D97-43CB7AEA0142"), PaymentMethods[AlipayPayment]) },
                { BankTransfer, (new("031FD0E3-A377-48E4-BE11-B52E8B3AD36B"), PaymentMethods[AlipayPayment]) },
                { WeChatPayScan, (new("450C18AD-214E-4ED1-AAF9-6BAE74BF0BA5"), PaymentMethods[WeChatPayPayment]) }
            };

        private async Task GeneratePaymentAsync()
        {
            await GeneratePaymentsAsync();
            await GeneratePaymentMethodsAsync();
            await GeneratePaymentChannelsAsync();
            await GeneratePaymentMethodChannelsAsync();

            await _repository.SaveChangesAsync();
            await GenerateBrandPaymentsAsync();
        }

        private Task GeneratePaymentsAsync()
        {
            var payments = Payments.Select(p => new Payment
            {
                Id = p.Value,
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

        private Task GeneratePaymentMethodsAsync()
        {
            var paymentMethods = PaymentMethods.Select(p => new PaymentMethod
            {
                Id = p.Value.Item1,
                Code = p.Key,
                Description = "online payment method",
            }).ToArray();

            return _repository.PaymentMethods.AddRangeAsync(paymentMethods);
        }

        private Task GeneratePaymentChannelsAsync()
        {
            var paymentVendors = PaymentChannels.Select(p => new PaymentChannel
            {
                Id = p.Value.Item1,
                Code = p.Key,
                Description = "online payment channel",
            }).ToArray();

            return _repository.PaymentChannels.AddRangeAsync(paymentVendors);
        }

        private Task GeneratePaymentMethodChannelsAsync()
        {
            var query = from payment in Payments
                        from method in PaymentMethods
                        from channel in PaymentChannels
                        select new PaymentMethodChannel
                        {
                            Id = Guid.NewGuid(),
                            PaymentId = payment.Value,
                            MethodId = method.Value.Item1,
                            ChannelId = channel.Value.Item1,
                            CreatedByUsername = AdminUsername,
                            CreatedOn = DateTimeOffset.Now,
                            UpdatedByUsername = AdminUsername,
                            UpdatedOn = DateTimeOffset.Now
                        };

            return _repository.PaymentMethodChannels.AddRangeAsync(query);
        }

        private async Task GenerateBrandPaymentsAsync()
        {
            var allPaymentMethodChannels = await _repository.PaymentMethodChannels.ToArrayAsync();
            var allBrands = Resellers
                .SelectMany(x => x.Licensees.SelectMany(l => l.Brands))
                .Distinct();

            var brandPayments = from brand in allBrands
                                from pmc in allPaymentMethodChannels
                                let brandPaymentId = GuidExtensions.NewSequentialGuid()
                                select new BrandPayment
                                {
                                    Id = brandPaymentId,
                                    DisplayOrder = 1,
                                    BrandId = brand.Id,
                                    PaymentMethodChannelId = pmc.Id,
                                    TransferType = PaymentTransferType.Deposit,
                                    ChannelName = "阿里pay",
                                    PaymentStatus = SwitchStatus.Enabled,
                                    TransferLimitFrom = 10,
                                    TransferLimitTo = 100000,
                                    PlayerListCondition = PlayerListCondition.All,
                                    AllPlayers = true,
                                    IdentityAuthentication = true,
                                    PhoneAuthentication = true,
                                    DepositedAmountUnlimited = true,
                                    DepositedAmountMin = 100,
                                    DepositedAmountMax = 1000,
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
                                    ExtendedData = JsonSerializer.Serialize(
                                        new
                                        {
                                            BankAccount = "987654321",
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

            await _repository.BrandPayments.AddRangeAsync(brandPayments);
        }
    }
}
