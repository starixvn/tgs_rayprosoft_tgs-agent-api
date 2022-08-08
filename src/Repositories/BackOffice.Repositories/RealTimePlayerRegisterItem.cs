using System;

namespace TGS.BackOffice.Repositories;
internal record RealTimePlayerRegisterItem(
    string PlayerCode,
    string UserName,
    Guid AgentId,
    DateTimeOffset RegisteredOn,
    DateTimeOffset? LastLoginOn,
    int? LoginCount);
