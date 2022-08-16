using System;

namespace TGS.Services.Operator;

public record RouletteResultWithCount(
    Guid? Id,
    int ItemIndex,
    int RouletteCount);
