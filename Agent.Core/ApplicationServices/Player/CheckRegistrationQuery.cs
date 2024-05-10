using System;
using System.Text.Json;

namespace TGS.Services.Agent.ApplicationServices.Player;

public record CheckRegistrationQuery(Guid BrandId, string Username, JsonDocument Data);