using System;

namespace TGS.Services.Agent.ApplicationServices.Player;

public record SetPlayerEmailCommand(string PlayerCode, string Email);
