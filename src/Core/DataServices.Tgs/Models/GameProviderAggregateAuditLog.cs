using System;

namespace TGS.Core.DataServices.Tgs.Models;

public class GameProviderAggregateAuditLog
{
    public int Id { get; set; }

    public Guid? GameProviderId { get; set; }

    public Guid CreatedByUserId { get; set; }

    public DateTimeOffset CreatedOn { get; set; }
}