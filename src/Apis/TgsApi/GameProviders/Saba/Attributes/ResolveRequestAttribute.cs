using System;

namespace TGS.Apis.TgsApi.GameProviders.Saba.Attributes
{
    public class ResolveRequestAttribute : Attribute
    {
        public Type Type { get; init; }
    }
}