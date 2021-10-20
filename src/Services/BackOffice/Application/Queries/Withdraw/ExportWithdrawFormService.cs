using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment.Clients;
using TGS.Core.Infrastructures.Utility;
using TGS.Services.BackOffice.Application.Models.Withdraw;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Queries.Withdraw
{
    internal class ExportWithdrawFormService : IGetService<ExportWithdrawForm, MemoryStream>
    {
        private readonly IPaymentServiceClient _paymentServiceClient;

        public ExportWithdrawFormService(IPaymentServiceClient paymentServiceClient)
        {
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
        }

        public async ValueTask<MemoryStream> GetAsync(ExportWithdrawForm query, CancellationToken cancellationToken)
        {
            var result = await _paymentServiceClient.QueryWithdrawFormsAsync(
                new WithdrawFormQueryConditions
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
                    ShowAll = true,
                    SortBy = query.Paging.SortBy
                }).ConfigureAwait(false);

            var resultPaymentMethods = await _paymentServiceClient.GetAllPaymentMethodsAsync().ToDictionaryAsync(x => x.Id).ConfigureAwait(false);

            var resource = new ResourceManager(typeof(Resources.Withdraw));

            var exportData = result.Items?.Select((x, index) => new ExportWithdrawModel
            {
                Sequence = index + 1,
                Username = x.UserName,
                UseType = resource.GetString(x.UserType.ToString()) ?? x.UserType.ToString(),
                Id = x.Id.ToString(),
                ThirdPartyOrderNo = x.ThirdPartyOrderNo,
                Amount = x.Amount,
                ApplicationTime = x.CreatedOn,
                UpdateTime = x.UpdatedOn,
                Status = resource.GetString(x.Status.ToString()) ?? x.Status.ToString(),
                Remark = x.Remark
            });

            var xlsx = exportData.ExportWithdrawLocalization("Withdraw", result.Sum, resource);

            await using var stream = new MemoryStream();

            xlsx.SaveAs(stream);

            return new MemoryStream(stream.ToArray());
        }
    }
}
