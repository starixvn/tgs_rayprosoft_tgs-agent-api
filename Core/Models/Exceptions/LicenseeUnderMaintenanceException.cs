using System;

namespace TGS.Core.Models.Exceptions
{
    public sealed class LicenseeUnderMaintenanceException : Exception
    {
        public LicenseeUnderMaintenanceException() : base("Licensee is put into maintenance")
        {
        }

        public LicenseeUnderMaintenanceException(string message, Exception inner = null) : base(message, inner)
        {
        }
    }
}