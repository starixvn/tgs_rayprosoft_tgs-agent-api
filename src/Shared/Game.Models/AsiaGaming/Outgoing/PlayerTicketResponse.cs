using System.Xml.Serialization;

namespace TGS.Shared.Game.Models.AsiaGaming.Outgoing
{
    [XmlRoot]
    public record PlayerTicketResponse
    {
        [XmlElement(ElementName = "ResponseCode")]
        public string Error { get; init; }
    }
}