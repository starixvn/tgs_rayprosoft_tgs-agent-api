using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Exceptions
{
    public sealed class PlayerDisabledException : Exception
    {
        public PlayerStatus DisabledStatus { get; }

        public PlayerDisabledException(PlayerStatus status) : base($"Player disabled with status : {status}")
        {
            DisabledStatus = status;
        }
    }
}