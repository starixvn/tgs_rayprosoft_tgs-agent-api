using System;

namespace TGS.Core.DataServices.Tgs.Models;

public class BrandMaintenance
{
    public TimeOnly BeginTime { get; set; }

    public virtual Brand Brand { get; set; }

    public Guid BrandId { get; set; }

    public string Content { get; set; }

    public TimeOnly EndTime { get; set; }

    public Guid Id { get; set; }
}
