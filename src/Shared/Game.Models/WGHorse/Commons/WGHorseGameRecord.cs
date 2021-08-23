using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.WGHorse.Commons
{
    public record WGHorseGameRecord
    {
        [JsonPropertyName("memberCode")]
        public string MemberCode { get; set; }
        [JsonPropertyName("recordDate")]
        public string RecordDate { get; set; }
        [JsonPropertyName("batchNo")]
        public string BatchNo { get; set; }
        [JsonPropertyName("showType")]
        public string ShowType { get; set; }
        [JsonPropertyName("showDiscount")]
        public string ShowDiscount { get; set; }
        [JsonPropertyName("showContent")]
        public string ShowContent { get; set; }
        [JsonPropertyName("showResult")]
        public string ShowResult { get; set; }
        [JsonPropertyName("showAmount")]
        public string ShowAmount { get; set; }
        [JsonPropertyName("showWin")]
        public string ShowWin { get; set; }
        [JsonPropertyName("showOddsWin")]
        public string ShowOddsWin { get; set; }
        [JsonPropertyName("showDivideDate")]
        public string ShowDivideDate { get; set; }
        [JsonPropertyName("showRaceDate")]
        public string ShowRaceDate { get; set; }
        [JsonPropertyName("ArrayInvoiceChild")]
        public object[] ArrayInvoiceChild { get; set; }
    }
}
