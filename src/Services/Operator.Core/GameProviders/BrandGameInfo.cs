using System;
using System.Collections.Generic;
using System.Text.Json;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operator
{
    public record BrandGameInfo(
        int Id,
        string Code,
        string Name,
        string Description,
        string SubGpCode,
        Guid GpId,
        string GpCode,
        GameType Type,
        TagInfo Tag,
        int DisplayOrder,
        IDictionary<string, JsonDocument>? Localization);
}
