using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
    }    

}
