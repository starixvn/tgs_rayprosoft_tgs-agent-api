using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.BackendOffice.DataService.Models;

namespace TGS.Apis.BackendOffice.DataService.DataInitializers
{
    public partial class DataInitializer
    {
        private const string Help2Pay = "Help2Pay";

        private const string AlipayPayment = "Alipay";
        private const string WeChatPayPayment = "WeChatPay";

        private const string AlipayScan = "AlipayScan";
        private const string WeChatPayScan = "WeChatPayScan";
        private const string BankTransfer = "BankTransfer";


        private static readonly Dictionary<string, Guid> PaymentVendors = new Dictionary<string, Guid>
        {
            {Help2Pay, new Guid("30CB4D17-2506-4D72-8470-65063B80F9C9")}
        };

        private static readonly Dictionary<string, (Guid, Guid)> PaymentMethods = new Dictionary<string, (Guid, Guid)>
        {
            {AlipayPayment, (new Guid("67252F3A-6826-45CE-8BBC-C53E2CA7CDCB"), PaymentVendors[Help2Pay])},
            {WeChatPayPayment, (new Guid("083D8754-1FE3-478A-9417-80DA796C3450"), PaymentVendors[Help2Pay])}
        };

        private readonly Dictionary<string, (Guid, (Guid, Guid))> _paymentChannels =
            new Dictionary<string, (Guid, (Guid, Guid))>
            {
                {
                    AlipayScan,
                    (new Guid("E761A2D3-5705-417F-8D97-43CB7AEA0142"), PaymentMethods[AlipayPayment])
                },
                {
                    BankTransfer,
                    (new Guid("031FD0E3-A377-48E4-BE11-B52E8B3AD36B"), PaymentMethods[AlipayPayment])
                },
                {
                    WeChatPayScan,
                    (new Guid("450C18AD-214E-4ED1-AAF9-6BAE74BF0BA5"), PaymentMethods[WeChatPayPayment])
                }
            };

        private Task GeneratePaymentVendorsAsync()
        {
            var paymentVendors = PaymentVendors.Select(p => new PaymentVendor
            {
                Id = p.Value,
                Code = p.Key,
                Description = "online payment platform",
                CreatedByUsername = AdminUsername,
                CreatedOn = DateTimeOffset.Now,
                UpdatedByUsername = AdminUsername,
                UpdatedOn = DateTimeOffset.Now
            }).ToArray();
            return _repository.BulkInsertAsync(paymentVendors);
        }

        private Task GeneratePaymentMethodsAsync()
        {
            var paymentMethods = PaymentMethods.Select(p => new PaymentMethod
            {
                Id = p.Value.Item1,
                Code = p.Key,
                PaymentVendorId = p.Value.Item2,
                Description = "online payment method",
                CreatedByUsername = AdminUsername,
                CreatedOn = DateTimeOffset.Now,
                UpdatedByUsername = AdminUsername,
                UpdatedOn = DateTimeOffset.Now
            }).ToArray();
            return _repository.BulkInsertAsync(paymentMethods);
        }

        private Task GeneratePaymentChannelsAsync()
        {
            var paymentVendors = _paymentChannels.Select(p => new PaymentChannel
            {
                Id = p.Value.Item1,
                Code = p.Key,
                Description = "online payment channel",
                PaymentVendorId = p.Value.Item2.Item2,
                PaymentMethodId = p.Value.Item2.Item1,
                CreatedByUsername = AdminUsername,
                CreatedOn = DateTimeOffset.Now,
                UpdatedByUsername = AdminUsername,
                UpdatedOn = DateTimeOffset.Now
            }).ToArray();
            return _repository.BulkInsertAsync(paymentVendors);
        }
    }
}