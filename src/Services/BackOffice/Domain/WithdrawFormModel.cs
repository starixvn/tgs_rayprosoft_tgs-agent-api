using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TGS.Services.BackOffice.Domain
{
    public record WithdrawFormModel(
        Guid Id,
        string PlayerCode,
        string UserName,
        Guid BrandId,
        Guid? BrandPaymentId,
        string ChannelName,
        Guid? PaymentMethodId,
        decimal Amount,
        string Remark,
        int Status,
        DateTimeOffset CreatedOn,
        DateTimeOffset UpdatedOn);
}
