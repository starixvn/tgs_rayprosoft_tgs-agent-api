using System;

namespace TGS.Core.Infrastructures.Log.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SuppressApiLogAttribute : Attribute
    {
    }
}
