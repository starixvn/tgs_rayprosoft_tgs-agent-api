using System;

namespace TGS.Services.Agent.Exceptions;

public class PlayerIsExistsException : Exception
{
    public PlayerIsExistsException(string info)
        : base($"The ({info}) is Existed.")
    {
    }
}
