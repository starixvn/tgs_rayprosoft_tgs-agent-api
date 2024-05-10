namespace TGS.Apis.AgentApi.Models;

public record SetPlayerPhoneRequest(string PlayerCode, string AreaCode, string Phone);