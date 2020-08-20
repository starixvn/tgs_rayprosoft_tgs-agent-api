using System;

namespace TGS.Core.Models.Generics
{
    public class Map<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }
    }
}
