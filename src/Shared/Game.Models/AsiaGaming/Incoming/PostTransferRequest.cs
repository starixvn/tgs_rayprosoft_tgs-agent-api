using System.Xml.Serialization;
using TGS.Shared.Game.Models.AsiaGaming.Enums;

namespace TGS.Shared.Game.Models.AsiaGaming.Incoming
{
    [XmlRoot(ElementName = "Data")]
    public record PostTransferRequest
    {
        [XmlElement(ElementName = "Record")] 
        public TransferRecord Record { get; init; }
    }

    public record TransferRecord
    {
        [XmlElement(ElementName = "sessionToken")]
        public string SessionToken { get; init; }
        
        [XmlElement(ElementName = "currency")]
        public string Currency { get; init; }
        
        [XmlElement(ElementName = "value")]
        public decimal Amount { get; init; }

        [XmlElement(ElementName = "validBetAmount")]
        public decimal BetAmount { get; init; }

        [XmlElement(ElementName = "netAmount")]
        public decimal WinAmount { get; init; }

        [XmlElement(ElementName = "playname")]
        public string PlayerCode { get; init; }
        
        [XmlElement(ElementName = "agentCode")]
        public string AgentCode { get; init; }
        
        [XmlElement(ElementName = "betTime")]
        public string BetTime { get; init; }
        
        [XmlElement(ElementName = "settletime")]
        public string SettleTime { get; init; }

        [XmlElement(ElementName = "transactionID")]
        public string[] TransactionIds { get; init; }
        
        [XmlIgnore]
        public string TransactionId => TransactionIds[0];

        [XmlElement(ElementName = "billNo")]
        public string BillNo { get; init; }

        [XmlElement(ElementName = "platformType")]
        public string GamePlatformType { get; init; }

        [XmlElement(ElementName = "round")]
        public string GameRound { get; init; }

        [XmlElement(ElementName = "gametype")]
        public string GameType { get; init; }
        
        [XmlElement(ElementName = "gameCode")]
        public string RoundId { get; init; }
        
        [XmlElement(ElementName = "tableCode")]
        public string TableCode { get; init; }
        
        [XmlElement(ElementName = "transactionType")]
        public TransactionType TransactionType { get; init; }
        
        [XmlElement(ElementName = "transactionCode")]
        public string TransactionCode { get; init; }
        
        [XmlElement(ElementName = "deviceType")]
        public string DeviceType { get; init; }
        
        [XmlElement(ElementName = "playtype")]
        public string PlayType { get; init; }

        [XmlElement(ElementName = "finish")]
        public string RoundFinish { get; init; }

        [XmlElement(ElementName = "gameResult")]
        public string GameResult { get; init; }
    }
}