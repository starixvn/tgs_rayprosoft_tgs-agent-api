using System;

#nullable enable

namespace TGS.Core.DataServices.Tgs.Models;

public class PlayerBankInfoTag
{
    public Guid BankInfoId { get; set; }

    public Guid BrandId { get; set; }

    public string PlayerCode { get; set; } = default!;

    public string Tag { get; set; } = default!;

    public string TagType { get; set; } = default!;
}
