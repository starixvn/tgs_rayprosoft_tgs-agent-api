using System.Xml.Serialization;

namespace TGS.Shared.Game.Models.AsiaGaming.Outgoing
{
    [XmlRoot(ElementName = "result")]
    public record CheckOrCreateAccountResponse
    {
        [XmlAttribute(AttributeName = "info")]
        public string Result { get; init; }

        [XmlAttribute(AttributeName = "msg")]
        public string Message { get; init; }
    }
}