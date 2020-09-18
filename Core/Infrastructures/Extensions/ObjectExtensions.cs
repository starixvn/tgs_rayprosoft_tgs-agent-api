using System.Text.Json;

namespace TGS.Core.Infrastructures.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object obj, bool indented = false)
        {
            return JsonSerializer.Serialize(obj, new() { WriteIndented = indented });
        }
    }
}
