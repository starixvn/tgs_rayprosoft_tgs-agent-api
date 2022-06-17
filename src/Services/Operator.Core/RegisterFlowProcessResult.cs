using System.Text.Json;

namespace TGS.Services.Operator;

public record RegisterFlowProcessResult(
    bool IsCompleted,
    string? NextStepType,
    JsonDocument? StepResponse);
