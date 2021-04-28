using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.KY.Outgoing
{
    public record KYApiRequest
    {
        /// <summary>
        /// Gets or sets the agent.
        /// </summary>
        /// <value>
        /// The agent.
        /// </value>
        [JsonPropertyName("agent")]
        public string Agent { get; init; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; init; }

        /// <summary>
        /// Gets or sets the parameter.
        /// </summary>
        /// <value>
        /// The parameter.
        /// </value>
        [JsonPropertyName("param")]
        public string Param { get; init; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        [JsonPropertyName("key")]
        public string Key { get; set; }

    }
}
