using System;
using System.Globalization;

namespace TGS.Services.Agent.ApplicationServices.Agent;

public record ForgotPasswordCommand(Guid BrandId, CultureInfo CultureInfo, string EmailOrPhone);
