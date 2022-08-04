using System;

#nullable enable

namespace TGS.Apis.BackendOffice.Controllers.Messages.Promotion;

public class RouletteItemViewModel
{
    public decimal Bonus { get; set; }

    public Guid Id { get; set; }

    public string Title { get; set; } = default!;
}
