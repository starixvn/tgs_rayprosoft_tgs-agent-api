using System;

namespace TGS.Infrastructures.Log.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SuppressApiLogAttribute : Attribute
    {
    }
}
