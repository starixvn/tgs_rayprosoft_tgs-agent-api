using System;

namespace TGS.Core.Models.Exceptions
{
    public class GameUnderMaintenanceException : Exception
    {
        public GameUnderMaintenanceException() : base("Game is put into maintenance") { }
    }
}