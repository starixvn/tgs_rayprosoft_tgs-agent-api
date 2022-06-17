using TGS.Core.Models.Services.Games;

#nullable enable

namespace TGS.Services.GameOperations;

public record GameActionMessage(bool isSettle, BetData BetData, bool isBatch = false, BatchBetData? BatchBetData = null);
