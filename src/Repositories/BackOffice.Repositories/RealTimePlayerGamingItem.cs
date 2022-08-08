using System;

namespace TGS.BackOffice.Repositories;
internal record RealTimePlayerGamingItem(
    string PlayerCode,
    string PlayerName,
    Guid AgentId,
    string GpCode,
    string SubGpCode,
    decimal WinLossSummary);
