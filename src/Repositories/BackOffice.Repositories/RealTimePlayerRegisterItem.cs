using System;

namespace TGS.BackOffice.Repositories;
internal record RealTimePlayerRegisterItem(
    string PlayerCode,
    string UserName,
    Guid AgentId,
    DateTime RegisteredOn,
    DateTime LastLoginOn,
    int LoginCount);
