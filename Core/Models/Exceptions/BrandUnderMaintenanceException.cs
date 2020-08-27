using System;

namespace TGS.Core.Models.Exceptions
{
    public class BrandUnderMaintenanceException : Exception
    {
        public BrandUnderMaintenanceException() : base("Brand is put into maintenance")
        {
        }

        public BrandUnderMaintenanceException(string message, Exception inner = null) : base(message, inner)
        {
        }
    }
}