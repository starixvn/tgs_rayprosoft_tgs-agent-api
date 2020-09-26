using System.Runtime.Serialization;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Messages
{
    [DataContract]
    public record TgsResponse
    {
        [DataMember(EmitDefaultValue = false)]
        public int Error { get; init; } = (int) TgsApiError.None;

        [DataMember(EmitDefaultValue = false)]
        public string ErrorMessage { get; init; }
    }
}
