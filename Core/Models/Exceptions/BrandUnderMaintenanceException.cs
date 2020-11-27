using System;

namespace TGS.Core.Models.Exceptions
{
    public sealed class BrandUnderMaintenanceException : Exception
    {
        public BrandUnderMaintenanceException() : base("Licensee is put into maintenance")
        {
        }

        public BrandUnderMaintenanceException(string message, Exception inner = null) : base(message, inner)
        {
        }
    }
}