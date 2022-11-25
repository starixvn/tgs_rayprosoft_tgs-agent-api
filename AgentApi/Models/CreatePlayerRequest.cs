using System;
using MediatR;

namespace TGS.Apis.AgentApi.Models;

public record CreatePlayerRequest(string Username, string Password);
