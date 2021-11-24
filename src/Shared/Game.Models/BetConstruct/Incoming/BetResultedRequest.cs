using System;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.BetConstruct.Incoming
{
    public record BetResultedRequest : BaseRequest
    {
        /// <summary>
        /// Unique transaction ID in BetConstructBE to identify  each call to OperatorBE regarding to bets. This is not a  bet ID. This ID should be kept by OperatorBE to
        /// </summary>
        [JsonPropertyName("TransactionId")]
        public long TransactionId { get; set; }

        /// <summary>
        /// Real Amount in User’s currency. OperatorBE can  compare this amount to the previously called  BetResulted Amount and increase or decrease User’s  balance. OperatorBE can store this value on the bet’s  record as the final total amount returned to User’s  balance from this bet. (Mandatory)
        /// </summary>
        [JsonPropertyName("Amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Unique ID of the bet in BetConstructBE to identify the  bet. This value should be referenced when talking to  LiveChat and other support services at BetConstruct. .  (Mandatory)
        /// </summary>
        [JsonPropertyName("BetId")]
        public long BetId { get; set; }

        /// <summary>
        /// The resulted state of the bet. (Mandatory) 1 – Accepted (not resulted)
        /// 2 – Returned
        /// 3 – Lost
        /// 4 – Won
        /// 5 – Cashed-out
        /// </summary>
        [JsonPropertyName("BetState")]
        public int BetState { get; set; }

        /// <summary>
        /// Bonus amount in User’s currency won from this bet.  When the bet is a wagering bonus bet, the won  amount goes into this field. See Bonus API for more  details.
        /// </summary>
        [JsonPropertyName("BonusAmount")]
        public decimal BonusAmount { get; set; }

        /// <summary>
        /// User’s bonus ID used for this bet. If the User’s bonus  was created by GetBonusDetails API call, the value is  the ExternalBonusId received from GetClientDetails  call.
        /// </summary>
        [JsonPropertyName("BonusId")]
        public int BonusId { get; set; }

        /// <summary>
        /// The UTC time that the bet was settled.
        /// </summary>
        [JsonPropertyName("CalcDate")]
        public DateTimeOffset CalcDate { get; set; }

        /// <summary>
        /// True – when the selection is on live match, False – Pre-Match.
        /// </summary>
        [JsonPropertyName("IsLive")]
        public bool IsLive { get; set; }

        /// <summary>
        /// [{"SelectionId":123,"State":1}]
        /// Possible values for State field are
        /// NotResulted = 0,
        /// Returned = 2
        /// Lost = 3
        /// Won = 4
        /// WinReturn = 5
        /// LossReturn = 6
        /// SelectionId will indicate the selection Id within the  current bet.
        /// </summary>
        [JsonPropertyName("BetSelectionState")]
        public ResultedSelection BetSelectionState { get; set; }

        /// <summary>
        /// True - when the bet is cancelled/voided otherwise  False
        /// </summary>
        [JsonPropertyName("IsVoid")]
        public bool IsVoid { get; set; }

        /// <summary>
        /// True - when the bet has been resettled otherwise False  or absent
        /// </summary>
        [JsonPropertyName("IsResettled")]
        public bool IsResettled { get; set; }

        /// <summary>
        /// True - when there was a partial cashout, indicating  that this is a preliminary result, otherwise False or absent
        /// </summary>
        [JsonPropertyName("IsPartial")]
        public bool IsPartial { get; set; }
    }
}
