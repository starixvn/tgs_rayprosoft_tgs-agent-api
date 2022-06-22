using System;

#nullable enable

namespace TGS.Core.DataServices.Tgs.Models;

public class PlayerProperty
{
    public Guid BrandId { get; set; }

    public string Name { get; set; } = default!;

    public string PlayerCode { get; set; } = default!;

    public string Value { get; set; } = default!;
}
