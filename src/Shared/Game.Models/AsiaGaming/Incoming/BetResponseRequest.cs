using System.Xml.Serialization;

namespace TGS.Shared.Game.Models.AsiaGaming.Incoming
{
    [XmlRoot(ElementName = "Data")]
    public record BetResponseRequest
    {
        [XmlElement(ElementName = "betResponse")]
        public string BetResponse { get; init; }

        [XmlElement(ElementName = "sessionToken")]
        public string SessionToken { get; init; }
        
        [XmlElement(ElementName = "playname")]
        public string PlayerCode { get; init; }
        
        [XmlElement(ElementName = "transactionID")]
        public string TransactionId { get; init; }
        
        [XmlElement(ElementName = "record")] 
        public BetRecord[] Records { get; init; }
    }

    public record BetRecord
    {
        [XmlElement(ElementName = "billNo")]
        public string BillNo { get; set; }
        
        [XmlElement(ElementName = "value")]
        public decimal Amount { get; init; }
        
        [XmlElement(ElementName = "playtype")]
        public string PlayType { get; init; }

        [XmlElement(ElementName = "val")]
        public string PlayVal { get; init; }
    }
}