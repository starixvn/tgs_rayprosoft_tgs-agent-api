using System.Xml.Serialization;
using TGS.Shared.Game.Models.AsiaGaming.Enums;

namespace TGS.Shared.Game.Models.AsiaGaming.Incoming
{
    [XmlRoot(ElementName = "Data")]
    public record PostTransferRequest
    {
        [XmlElement(ElementName = "Record")] 
        public TransferRecord Record { get; set; }
    }

    public record TransferRecord
    {
        [XmlElement(ElementName = "sessionToken")]
        public string SessionToken { get; set; }
        
        [XmlElement(ElementName = "currency")]
        public string Currency { get; set; }
        
        [XmlElement(ElementName = "value")]
        public decimal Amount { get; set; }

        [XmlElement(ElementName = "validBetAmount")]
        public decimal BetAmount { get; set; }

        [XmlElement(ElementName = "netAmount")]
        public decimal WinAmount { get; set; }

        [XmlElement(ElementName = "playname")]
        public string PlayerCode { get; set; }
        
        [XmlElement(ElementName = "agentCode")]
        public string AgentCode { get; set; }
        
        [XmlElement(ElementName = "betTime")]
        public string BetTime { get; set; }
        
        [XmlElement(ElementName = "settletime")]
        public string SettleTime { get; set; }

        [XmlElement(ElementName = "transactionID")]
        public string TransactionId { get; set; }

        [XmlElement(ElementName = "billNo")]
        public string BillNo { get; set; }

        [XmlElement(ElementName = "platformType")]
        public string GamePlatformType { get; set; }

        [XmlElement(ElementName = "round")]
        public string GameRound { get; set; }

        [XmlElement(ElementName = "gametype")]
        public string GameType { get; set; }
        
        [XmlElement(ElementName = "gameCode")]
        public string RoundId { get; set; }
        
        [XmlElement(ElementName = "tableCode")]
        public string TableCode { get; set; }
        
        [XmlElement(ElementName = "transactionType")]
        public TransactionType TransactionType { get; set; }
        
        [XmlElement(ElementName = "transactionCode")]
        public string TransactionCode { get; set; }
        
        [XmlElement(ElementName = "deviceType")]
        public string DeviceType { get; set; }
        
        [XmlElement(ElementName = "playtype")]
        public string PlayType { get; set; }

        [XmlElement(ElementName = "finish")]
        public string RoundFinish { get; set; }

        [XmlElement(ElementName = "gameResult")]
        public string GameResult { get; set; }
    }
}