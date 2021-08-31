using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.WGSports.Enums;

namespace TGS.Shared.Game.Models.WGSports.Outgoing
{
    public record WGSportsGameRecordRequest
    {
        /// <summary>
        /// The game record time
        /// </summary>
        public GameRecordTime GameRecordTime { get; init; }

        /// <summary>
        /// The start time
        /// </summary>
        public string StartTime { get; init; }

        /// <summary>
        /// The end time
        /// </summary>
        public string EndTime { get; init; }
    }
}
