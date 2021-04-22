using System.Xml.Serialization;

namespace TGS.Shared.Game.Models.AsiaGaming.Outgoing
{
    [XmlRoot]
    public class PlayerTicketResponse
    {
        [XmlElement(ElementName = "ResponseCode")]
        public string Error { get; set; }
    }
}