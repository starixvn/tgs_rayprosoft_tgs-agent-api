using System;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.Core.Infrastructures.Utility;
using Tgs.Payment.Clients;
using TGS.Services.BackOffice.Application.Models.Deposit;

namespace TGS.Services.BackOffice.Application.Queries.Deposit
{
    public class ExportDepositFormService : IGetService<ExportDepositForm, MemoryStream>
    {
        private readonly IPaymentServiceClient _paymentServiceClient;

        public ExportDepositFormService(IPaymentServiceClient paymentServiceClient)
        {
            _paymentServiceClient =
                paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
        }

        public async ValueTask<MemoryStream> GetAsync(ExportDepositForm query, CancellationToken cancellationToken)
        {
            var result = await _paymentServiceClient.QueryDepositFormsAsync(
                 new DepositFormQueryConditions
                 {
                     BrandId = query.BrandId,
                     CreateOnRange = query.CreateOnRange,
                     UpdateOnRange = query.UpdateOnRange,
                 },
                 new Tgs.Payment.Clients.Paging
                 {
                     ShowAll = true
                 });

            var resultPaymentMethods = await _paymentServiceClient
                .GetAllPaymentMethodsAsync()
                .ToDictionaryAsync(x => x.Id)
                .ConfigureAwait(false);

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
                ApplicationTime = x.CreatedOn.ToOffset(new(8, 0, 0)).ToString("d"),
                UpdateTime = x.UpdatedOn.ToOffset(new(8, 0, 0)).ToString("d"),
                Status = resource.GetString(x.Status.ToString()) ?? x.Status.ToString(),
                Remark = x.Remark
            });

            var xlsx = exportData.ExportDepositLocalization("Deposit", result.Sum, resource);

            var stream = new MemoryStream();

            xlsx.SaveAs(stream);

            stream.Position = 0;

            return stream;
        }
    }
}
