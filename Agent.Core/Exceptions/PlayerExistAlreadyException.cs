using System;

namespace TGS.Services.Agent.Exceptions;

public class PlayerExistAlreadyException(string info) : Exception($"The ({info}) is Existed.");
