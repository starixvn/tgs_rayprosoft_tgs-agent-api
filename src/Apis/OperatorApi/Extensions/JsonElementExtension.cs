using System.Text.Json;

namespace TGS.Apis.OperatorApi.Extensions
{
    public static class JsonElementExtenstion
    {
        public static JsonElement? GetPropertyExtension(this JsonElement jsonElement, string propertyName)
        {
            if (jsonElement.TryGetProperty(propertyName, out var returnElement))
            {
                return returnElement;
            }

            return null;
        }
    }
}
