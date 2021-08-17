using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TGS.Apis.TgsApi.GameProviders.BigGaming
{
    public class BigGamingConsts
    {
        public static Dictionary<string, decimal> BullBullOddsMapper => new()
        {
            { "0xD0000001", 1 } //下注闲1平倍
                ,
            { "0xD0000002", 5 }//下注闲1翻倍
                ,
            { "0xD0000004", 1 }//下注闲2平倍
                ,
            { "0xD0000008", 5 }//下注闲2翻倍
                ,
            { "0xD0000010", 1 }//下注闲3平倍
                ,
            { "0xD0000020", 5 }//下注闲3翻倍
        };
    }
}
