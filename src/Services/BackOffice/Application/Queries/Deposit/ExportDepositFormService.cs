using System;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment.Clients;
using TGS.Core.Infrastructures.Utility;
using TGS.Services.BackOffice.Application.Models.Deposit;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Queries.Deposit
{
    public class ExportDepositFormService : IGetService<ExportDepositForm, MemoryStream>
    {
        private readonly IPaymentServiceClient _paymentServiceClient;

        public ExportDepositFormService(IPaymentServiceClient paymentServiceClient)
        {
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
        }

        public async ValueTask<MemoryStream> GetAsync(ExportDepositForm query, CancellationToken cancellationToken)
        {
            var result = await _paymentServiceClient.QueryDepositFormsAsync(
                 new DepositFormQueryConditions
                 {
                     BrandId = query.BrandId,
                     ChannelName = query.ChannelName,
                     CreateOnRange = query.CreateOnRange,
                     PaymentMethodId = query.PaymentMethodId,
                     Status = query.Status,
                     ThirdPartyOrderNo = query.ThirdPartyOrderNo,
                     FormId = query.FormId,
                     UpdateOnRange = query.UpdateOnRange,
                     UserName = query.UserName
                 },
                 new Tgs.Payment.Clients.Paging
                 {
                     IsDesc = query.Paging.IsDesc,
                     Page = query.Paging.Page,
                     Rows = query.Paging.Rows,
                     ShowAll = query.Paging.ShowAll,
                     SortBy = query.Paging.SortBy
                 });

            var resultPaymentMethods = await _paymentServiceClient.GetAllPaymentMethodsAsync().ToDictionaryAsync(x => x.Id).ConfigureAwait(false);

            var resource = new ResourceManager(typeof(Resources.Deposit));

            var exportData = result.Items?.Select((x, index) => new ExportDepositModel
            {
                Sequence = index + 1,
                Username = x.UserName,
                Id = x.Id.ToString(),
                ThirdPartyOrderNo = x.ThirdPartyOrderNo,
                PaymentMethod = resource.GetString(resultPaymentMethods[x.PaymentMethodId].Name),
                PaymentName = x.ChannelName,
                Amount = x.Amount,
                ApplicationTime = x.CreatedOn,
                UpdateTime = x.UpdatedOn,
                Status = resource.GetString(x.Status.ToString()) ?? x.Status.ToString(),
                Remark = x.Remark
            });

            var xlsx = exportData.ExportDepositLocalization("Deposit", result.Sum, resource);

            await using var stream = new MemoryStream();

            xlsx.SaveAs(stream);

            return new MemoryStream(stream.ToArray());
        }

    }
}