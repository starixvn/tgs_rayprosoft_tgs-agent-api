using System;
using System.Globalization;

namespace TGS.Services.Agent.ApplicationServices.Agent;

public record ForgotPasswordByPhoneCommand(
    Guid BrandId,
    CultureInfo CultureInfo,
    string Phone);
