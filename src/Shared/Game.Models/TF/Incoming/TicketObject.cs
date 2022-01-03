using System;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.TF.Incoming
{
    public class TicketObject
    {
        /// <summary>
        /// 下注金额
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }

        /// <summary>
        /// 下注选项
        /// </summary>
        [JsonPropertyName("bet_selection")]
        public string BetSelection { get; init; }

        /// <summary>
        /// 盘口类型
        /// WIN = 主盘口独赢 (下注选项: home/away)
        /// 1X2 = 独赢(下注选项: home/draw/away)
        /// AH = 让分局(下注选项: home/away)
        /// OU = 大小(下注选项: over/under)
        /// OE = 单双(下注选项: odd/even)
        /// SPWINMAP = 局独赢(下注选项: home/away)
        /// WINMAP = 局独赢比分(下注选项: home/away)
        /// SPHA = 特别主客(下注选项: home/away)
        /// SPYN = 特别是否(下注选项: yes/no)
        /// SPOE = 特别单双(下注选项: odd/even)
        /// SPOU = 特别大小(下注选项: over/under)
        /// SP1X2 = 特别1X2(下注选项: home/draw/away)
        /// OR = 冠军盘(下注选项: 队伍名字)
        /// SPOR = 特别多项(下注选项: 自定)
        /// SPXX = 特别双项(下注选项: 自定)
        /// </summary>
        [JsonPropertyName("bet_type_name")]
        public string BetTypeName { get; init; }

        /// <summary>
        /// 比赛名称
        /// </summary>
        [JsonPropertyName("competition_name")]
        public string CompetitionName { get; init; }

        /// <summary>
        /// 货币
        /// </summary>
        [JsonPropertyName("currency")]
        public string Currency { get; init; }

        /// <summary>
        /// 下注时间
        /// </summary>
        [JsonPropertyName("date_created")]
        public DateTimeOffset DateCreated { get; init; }

        /// <summary>
        /// 输赢额
        /// WIN (赢) = 本金 (bet amount) + 盈利 (winnings)
        /// LOSS(输) = -本金(-bet amount)
        /// DRAW(和) = 本金(bet amount)
        /// PENDING(未结算) = null
        /// CANCELLED(取消) = null
        /// </summary>
        [JsonPropertyName("earnings")]
        public decimal? Earnings { get; init; }

        /// <summary>
        /// 欧盘赔率
        /// </summary>
        [JsonPropertyName("euro_odds")]
        public decimal EuroOdds { get; init; }

        /// <summary>
        /// 赛事开始时间
        /// </summary>
        [JsonPropertyName("event_datetime")]
        public DateTimeOffset EventDatetime { get; init; }

        /// <summary>
        /// 赛事ID
        /// </summary>
        [JsonPropertyName("event_id")]
        public int EventId { get; init; }

        /// <summary>
        /// 赛事名称
        /// </summary>
        [JsonPropertyName("event_name")]
        public string EventName { get; init; }

        /// <summary>
        /// 盘口名称
        /// </summary>
        [JsonPropertyName("game_market_name")]
        public string GameMarketName { get; init; }

        /// <summary>
        /// 游戏ID
        /// </summary>
        [JsonPropertyName("game_type_id")]
        public int GameTypeId { get; init; }

        /// <summary>
        /// 游戏名称
        /// </summary>
        [JsonPropertyName("game_type_name")]
        public string GameTypeName { get; init; }

        /// <summary>
        /// 让分数
        /// </summary>
        [JsonPropertyName("handicap")]
        public decimal? Handicap { get; init; }

        /// <summary>
        /// 注单号
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; init; }

        /// <summary>
        /// 是否连串
        /// </summary>
        [JsonPropertyName("is_combo")]
        public bool IsCombo { get; init; }

        /// <summary>
        /// true = 已重新结算
        /// </summary>
        [JsonPropertyName("is_unsettled")]
        public bool IsUnsettled { get; init; }

        /// <summary>
        /// 马来赔率
        /// Note: if it is combo, it will be null
        /// 备注：如果是连串，赔率会是null
        /// </summary>
        [JsonPropertyName("malay_odds")]
        public decimal? MalayOdds { get; init; }

        /// <summary>
        /// 第几局
        /// MAP 1 = 第一局
        /// Q1 = 第一节
        /// FIRST HALF - 上半场
        /// SECOND HALF - 下半场
        /// </summary>
        [JsonPropertyName("map_num")]
        public string MapNum { get; init; }

        /// <summary>
        /// 盘口局分
        /// match = 总局
        /// map = 局
        /// </summary>
        [JsonPropertyName("market_option")]
        public string MarketOption { get; init; }

        /// <summary>
        /// 会员号
        /// </summary>
        [JsonPropertyName("member_code")]
        public string MemberCode { get; init; }

        /// <summary>
        /// 会员下的赔率
        /// </summary>
        [JsonPropertyName("member_odds")]
        public decimal MemberOdds { get; init; }

        /// <summary>
        /// 会员下的盘
        /// euro
        /// hongkong
        /// indo
        /// malay
        /// </summary>
        [JsonPropertyName("member_odds_style")]
        public string MemberOddsStyle { get; init; }

        /// <summary>
        /// 更改时间
        /// </summary>
        [JsonPropertyName("modified_datetime")]
        public DateTimeOffset ModifiedDatetime { get; init; }

        /// <summary>
        /// 赔率
        /// Note: If it is combo, OR, 1x2, SPOR, it will be euro odds.Else it is malay odds
        /// 备注：如果是连串, OR, 1x2, SPOR，赔率是欧盘赔率。否则是马来赔率。
        /// </summary>
        [JsonPropertyName("odds")]
        public decimal Odds { get; init; }

        /// <summary>
        /// 下注的管道
        /// desktop-browser = 电脑浏览器
        /// mobile-browser = 手机浏览器(包括嵌入在APP里)
        /// mobile-app = 手机APP
        /// unknown = 未知
        /// null = 旧数据没有
        /// </summary>
        [JsonPropertyName("request_source")]
        public string RequestSource { get; init; }

        /// <summary>
        /// 盘口结果
        /// </summary>
        [JsonPropertyName("result")]
        public string Result { get; init; }

        /// <summary>
        /// 注单结果
        /// WIN = 赢
        /// LOSS = 输
        /// DRAW = 和
        /// CANCELLED = 取消
        /// </summary>
        [JsonPropertyName("result_status")]
        public string ResultStatus { get; init; }

        /// <summary>
        /// 结算时间
        /// </summary>
        [JsonPropertyName("settlement_datetime")]
        public DateTimeOffset? SettlementDatetime { get; init; }

        /// <summary>
        /// 注单状况
        /// confirmed = 确定
        /// settled = 结算
        /// cancelled = 取消
        /// </summary>
        [JsonPropertyName("settlement_status")]
        public string SettlementStatus { get; init; }

        /// <summary>
        /// 注单下注状况
        /// db = 早盘
        /// live = 滚球
        /// </summary>
        [JsonPropertyName("ticket_type")]
        public string TicketType { get; init; }
    }
}
