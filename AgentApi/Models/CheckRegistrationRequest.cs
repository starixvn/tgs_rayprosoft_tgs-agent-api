using System.Text.Json;

namespace TGS.Apis.AgentApi.Models;

public record CheckRegistrationRequest(string Username, JsonDocument Data);
