using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TGS.Services.BackOffice.Domain
{
    public record WithdrawFormInfo(
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
        JsonDocument ApplyInfo,
        JsonDocument PaymentReturn,
        DateTimeOffset CreatedOn,
        DateTimeOffset UpdatedOn);
}
