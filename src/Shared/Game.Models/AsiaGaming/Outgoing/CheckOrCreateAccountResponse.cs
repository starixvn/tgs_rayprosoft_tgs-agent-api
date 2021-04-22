using System.Xml.Serialization;

namespace TGS.Shared.Game.Models.AsiaGaming.Outgoing
{
    [XmlRoot(ElementName = "result")]
    public class CheckOrCreateAccountResponse
    {
        [XmlAttribute(AttributeName = "info")]
        public string Result { get; set; }

        [XmlAttribute(AttributeName = "msg")]
        public string Message { get; set; }
    }
}