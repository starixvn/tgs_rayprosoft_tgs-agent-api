using System;

namespace TGS.Core.DataServices.Tgs.Models;

public class BrandMaintenance
{
    public DateTimeOffset BeginTime { get; set; }

    public virtual Brand Brand { get; set; }

    public Guid BrandId { get; set; }

    public string Content { get; set; }

    public DateTimeOffset EndTime { get; set; }

    public Guid Id { get; set; }
}
