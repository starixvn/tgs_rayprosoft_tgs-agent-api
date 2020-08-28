using System;

namespace TGS.Core.Models.Exceptions
{
    public class GameProviderUnderMaintenanceException : Exception
    {
        public GameProviderUnderMaintenanceException() : base("Game provider is put into maintenance")
        {
        }
    }
}