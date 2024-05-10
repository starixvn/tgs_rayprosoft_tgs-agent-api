using System;
using System.Globalization;

namespace TGS.Services.Agent.ApplicationServices.Agent;

public record ForgotPasswordByEmailCommand(
    Guid BrandId,
    CultureInfo CultureInfo,
    string Email);