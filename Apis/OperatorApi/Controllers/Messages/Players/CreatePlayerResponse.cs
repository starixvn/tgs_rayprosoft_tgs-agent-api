using System.Runtime.Serialization;
using TGS.Core.Models.Messages;

namespace TGS.Apis.OperatorApi.Controllers.Messages.Players
{
    [DataContract, KnownType(typeof(TgsResponse))]
    public record CreatePlayerResponse : TgsResponse
    {
        [DataMember]
        public bool IsNew {  get; set; }

        [DataMember]
        public string PlayerCode {  get; set; }
    }
}
