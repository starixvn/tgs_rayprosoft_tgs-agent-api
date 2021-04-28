using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.KY.Outgoing
{
    /// <summary>
    /// 基地 - 請求
    /// </summary>
    public record KYBaseRequest
    {
        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        [JsonPropertyName("s")]
        public int Action { get; set; }
    }
}
