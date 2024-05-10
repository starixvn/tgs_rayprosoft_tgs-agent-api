using System;

namespace TGS.Services.Agent.Exceptions;

public class DividendPercentageException(string message) : Exception(message);