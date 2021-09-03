using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.TCG.Common;

namespace TGS.Shared.Game.Models.TCG.Outgoing
{
    public record TCGLaunchGameBySEARequest : TCGLaunchGameRequest
    {
        [Required]
        [JsonPropertyName("product_type")]
        public int ProductType => 384;
        [Required]
        [JsonPropertyName("lottery_bet_mode")]
        public string LotteryBetMode { get; init; } = "SEA_Tradition";
        [Required]
        [JsonPropertyName("view")]
        public string View { get; init; } = "Lobby";
        [Required]
        [JsonPropertyName("series")]
        public TCGSEASeries[] series { get; init; } = new TCGSEASeries[] {
            new TCGSEASeries{
                GameGroupCode="MAS",
                PrizeModeId=1
            },
            new TCGSEASeries{
                GameGroupCode="VNC",
                PrizeModeId=1
            },
            new TCGSEASeries{
                GameGroupCode="THAI",
                PrizeModeId=1
            },
            new TCGSEASeries{
                GameGroupCode="SGC",
                PrizeModeId=1
            },
            new TCGSEASeries{
                GameGroupCode="LAO",
                PrizeModeId=1
            }
        };
    }
}
