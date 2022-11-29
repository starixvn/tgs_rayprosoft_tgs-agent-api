using System;

namespace TGS.Services.Agent.Exceptions;

public class PlayerExistAlreadyException : Exception
{
    public PlayerExistAlreadyException(string info)
        : base($"The ({info}) is Existed.")
    {
    }
}
