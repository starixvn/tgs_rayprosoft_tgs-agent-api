using System;
using System.Collections.Generic;
using Tgs.Payment.Clients;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Queries.Deposit
{
    public record ExportDepositForm(
        Guid BrandId,
        (DateTimeOffset Begin, DateTimeOffset End)? CreateOnRange,
        (DateTimeOffset Begin, DateTimeOffset End)? UpdateOnRange);
}
