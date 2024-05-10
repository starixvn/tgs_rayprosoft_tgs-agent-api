using System;

namespace TGS.Services.Agent.Exceptions;

public class DividendConditionException(string? message = null) : Exception(message);
