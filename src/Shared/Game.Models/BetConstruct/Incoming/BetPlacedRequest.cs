using System;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.BetConstruct.Incoming
{
    public record BetPlacedRequest : BaseRequest
    {
        /// <summary>
        /// Unique transaction ID in BetConstructBE to identify  each call to OperatorBE regarding to bets. This is not a  bet ID. This ID should be kept by OperatorBE to  eliminate duplicate calls. It is also used in Rollback call  to cancel previous transactions. (Mandatory)
        /// </summary>
        [JsonPropertyName("TransactionId")]
        public long TransactionId { get; init; }

        /// <summary>
        /// Unique ID of the placed bet in BetConstructBE to  identify the bet. This value should be referenced when  talking to LiveChat and other support services at  BetConstruct. . (Mandatory)
        /// </summary>
        [JsonPropertyName("BetId")]
        public long BetId { get; init; }

        /// <summary>
        /// An amount (real money) in User’s currency placed on  the bet. (Mandatory)
        /// </summary>
        [JsonPropertyName("Amount")]
        public decimal Amount { get; init; }

        /// <summary>
        /// A UTC time the bet was placed. (Mandatory)
        /// </summary>
        [JsonPropertyName("Created")]
        public string Created { get; init; }

        /// <summary>
        /// 1 – Single/Ordinar, 2 – Multiple/Express, 3 – System,  4 - Chain, 5 - Trixie, 6 - Yankee, 7 - PermedYankee, 8  - SuperYankee, 9 - Heinz, 10 - SuperHeinz, 11 - Goliath, 12 - Patent, 13 - PermedPatent, 14 - Lucky15,  15 - Lucky31, 16 - Lucky63. (Mandatory)
        /// </summary>
        [JsonPropertyName("BetType")]
        public int BetType { get; init; }

        /// <summary>
        /// The count of sub-bet counts when System bet is  placed. Only for 'system', how much events were placed in  system 2 (of 3), for example.
        /// </summary>
        [JsonPropertyName("SystemMinCount")]
        public int? SystemMinCount { get; init; }

        /// <summary>
        /// Total price/odds/coefficient of the placed bet.
        /// </summary>
        [JsonPropertyName("TotalPrice")]
        public decimal TotalPrice { get; init; }

        /// <summary>
        /// The list of selections/events of the placed bet. Always 1  for Single bets, and more than 1 for Multiple and  System bets. This field should not be included in Hash  generation. (Mandatory) - at least one
        /// </summary>
        [JsonPropertyName("Selections")]
        public PlacedSelection[] Selections { get; init; }

        /// <summary>
        /// Bonus amount in User’s currency used to place the bet.  This field may be greater than 0 when bonus is used  on this bet, and some portion of the bet amount is  taken from the bonus amount. The field can be ignored  if Operator does not use Bonuses. See Bonus API for  more details.
        /// </summary>
        [JsonPropertyName("BonusBetAmount")]
        public decimal? BonusBetAmount { get; init; }

        /// <summary>
        /// User’s bonus ID used for this bet. If the User’s bonus  was created by GetBonusDetails API call, the value is  the ExternalBonusId received from GetClientDetails  call.
        /// </summary>
        [JsonPropertyName("BonusId")]
        public int? BonusId { get; init; }

        /// <summary>
        /// 42 - Web, 4 - Mobile, 16 - Android, 17 - iOS, 98 - Betshop, 99 - Terminal. The field is not included in  hash.
        /// </summary>
        [JsonPropertyName("Source")]
        public int Source { get; init; }

        /// <summary>
        /// Odd type that the player has used to place the bet.  0 - Decimal
        /// 1 - Fractional
        /// 2 - American
        /// 3 - HongKong
        /// 4 - Malay
        /// 5 - Indo
        /// (Optional)
        /// </summary>
        [JsonPropertyName("OddType")]
        public int OddType { get; init; }
    }
}
