using System.Xml.Serialization;
using TGS.Shared.Game.Models.AsiaGaming.Enums;

namespace TGS.Shared.Game.Models.AsiaGaming.Incoming
{
    [XmlRoot]
    public record TransferResponse
    {
        [XmlElement]
        public GameApiError ResponseCode { get; init; }

        [XmlElement]
        public decimal Balance { get; init; }
    }
}