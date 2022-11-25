using System;

namespace TGS.Services.Agent.ApplicationServices.Player;

public record SetPlayerPhoneCommand(string PlayerCode, string AreaCode, string Phone);
