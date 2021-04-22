using System.Xml.Serialization;

namespace TGS.Shared.Game.Models.AsiaGaming.Incoming
{
    [XmlRoot(ElementName = "Data")]
    public record BetResponseRequest
    {
        [XmlElement(ElementName = "betResponse")]
        public string BetResponse { get; set; }

        [XmlElement(ElementName = "sessionToken")]
        public string SessionToken { get; set; }
        
        [XmlElement(ElementName = "playname")]
        public string PlayerCode { get; set; }
        
        [XmlElement(ElementName = "transactionID")]
        public string TransactionId { get; set; }
        
        [XmlElement(ElementName = "record")] 
        public BetRecord[] Records { get; set; }
    }

    public record BetRecord
    {
        [XmlElement(ElementName = "billNo")]
        public string BillNo { get; set; }
        
        [XmlElement(ElementName = "value")]
        public decimal Amount { get; set; }
        
        [XmlElement(ElementName = "playtype")]
        public string PlayType { get; set; }

        [XmlElement(ElementName = "val")]
        public string PlayVal { get; set; }
    }
}