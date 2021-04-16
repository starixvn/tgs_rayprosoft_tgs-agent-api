using System;

namespace TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Attributes
{
    public class RequirePlayerTokenAttribute : Attribute
    {
        public bool IsRequired { get; init; }
    }
}