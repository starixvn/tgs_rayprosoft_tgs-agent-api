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
        private static readonly Dictionary<string, Guid> Payments = new()
        {
            { "Manual", new("30CB4D17-2506-4D72-8470-65063B80F9C9") }
        };

        private static readonly Dictionary<string, Guid> PaymentMethods = new()
        {
            { "BankTransfer", new("67252F3A-6826-45CE-8BBC-C53E2CA7CDCB") },
        };

        private static readonly Dictionary<string, Guid> PaymentChannels = new()
        {
            { "RichBank", new("E761A2D3-5705-417F-8D97-43CB7AEA0142") }
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
                Id = p.Value,
                Code = p.Key,
                Description = "online payment method",
            }).ToArray();

            return _repository.PaymentMethods.AddRangeAsync(paymentMethods);
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
            var query = from payment in Payments
                        from method in PaymentMethods
                        from channel in PaymentChannels
                        select new PaymentMethodChannel
                        {
                            Id = Guid.NewGuid(),
                            PaymentId = payment.Value,
                            MethodId = method.Value,
                            ChannelId = channel.Value,
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
                                    ChannelName = "PoorBank",
                                    PaymentStatus = SwitchStatus.Enabled,
                                    TransferLimitFrom = 10,
                                    TransferLimitTo = 100000,
                                    PlayerListCondition = PlayerListCondition.MatchAll,
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

            await _repository.BrandPayments.AddRangeAsync(brandPayments);
        }
    }
}
