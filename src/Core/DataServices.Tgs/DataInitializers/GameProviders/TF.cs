using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.TF;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    partial class DataInitializer
    {
        internal record TFGameProvider : IGameProvider
        {
            private readonly IWebHostEnvironment _env;
            private readonly IGame[] _games;

            public TFGameProvider(IWebHostEnvironment env)
            {
                _games = LoadGamesFromCsv(GameProviderConst.TFGaming)
                   .Select(g =>
                   {
                       if (g.EndpointPath.IsNotNullOrEmpty())
                       {
                           return g;
                       }

                       return (Game)g with { EndpointPath = string.Empty };
                   })
                   .ToArray();

                _env = env;
            }

            Guid IGameProvider.Id => new("67D16262-9915-3109-F18D-6043108712AA");

            string IGameProvider.Code => GameProviderConst.TFGaming;

            string IGameProvider.Name => "TF";

            IGameProviderConfiguration[] IGameProvider.GameProviderConfigurations =>
                new IGameProviderConfiguration[]
                {
                    new TFGameProviderConfiguration()
                };

            ICurrency[] IGameProvider.Currencies => new ICurrency[]
            {
                new HKDCurrency(),
                new IDRCurrency(),
                new IDR1000Currency(),
                new INRCurrency(),
                new JPYCurrency(),
                new KRWCurrency(),                
                new MYRCurrency(),
                new PHPCurrency(),
                new CNYCurrency("RMB"),
                new THBCurrency(),
                new VNDCurrency(),
                new VND1000Currency(),
                new USDCurrency(),
                new SGDCurrency(),
                new EURCurrency(),
            };

            ILanguage[] IGameProvider.Languages => new ILanguage[]
            {
                new CNLanguage("zh"),
                new TWLanguage("zh"),
                new THLanguage("th"),
                new VNLanguage("vn"),
                new KRLanguage("kr"),
                new JPLanguage("jp"),
                new USLanguage("en")
            };

            IGame[] IGameProvider.Games => _games;

            bool IGameProvider.AllowKickPlayer => true;

            bool IGameProvider.SupportTestPlayer => false;

            bool IGameProvider.TransferWallet => false;

            ushort IGameProvider.UniqueId => 24;

            GameProviderBetLimit[] IGameProvider.BetLimits => Array.Empty<GameProviderBetLimit>();

            SubGameProvider[] IGameProvider.SubGameProviders => new SubGameProvider[]
            {
                new SubGameProvider(GameProviderConst.TFGaming,"TF")
            };

            PluggableConfigurationBase IGameProvider.GetConfiguration() => new TFConfiguration
            {
                ServiceEndpointUrl =
                        _env.IsDocker() ? new Uri("http://game-tf") : new("http://localhost:62330"),
                BrandMerchants = new TFBrandMerchant[]
                {
                    new ()
                    {                        
                        LaunchUrl = new Uri("https://api-test-v4.diaoq.com"),
                        ApiUrl = new Uri("https://spi-test.r4espt.com"),
                        BrandCode = "Default",
                        BrandCurrency = "CNY",
                        PublicToken = "84778f10a5b147a4baf1db8d334ff266",
                        PrivateToken = "00023b230443b2b95ec3750ff324f04f7ff30dfc69c2dde6b4f0563ea8de135c412fc3645d2cb09ecaa3971fdf8ddde90f6b86fc4bbdade18863113de24d42e5b02c3440c339a3893611eefda81e92b8b3f504cedd44963c419f1691bd439927cb1b075700c1d4b9d09dbdef6022a4f196c6438cf1e90107a263309f7ea06072f6aa",
                        PartnerId = "2135"
                    },
                    new ()
                    {
                        LaunchUrl = new Uri("https://api-test-v4.diaoq.com"),
                        ApiUrl = new Uri("https://spi-test.r4espt.com"),
                        BrandCode = "GPI",
                        BrandCurrency = "CNY",
                        PublicToken = "84778f10a5b147a4baf1db8d334ff266",
                        PrivateToken = "00023b230443b2b95ec3750ff324f04f7ff30dfc69c2dde6b4f0563ea8de135c412fc3645d2cb09ecaa3971fdf8ddde90f6b86fc4bbdade18863113de24d42e5b02c3440c339a3893611eefda81e92b8b3f504cedd44963c419f1691bd439927cb1b075700c1d4b9d09dbdef6022a4f196c6438cf1e90107a263309f7ea06072f6aa",
                        PartnerId = "2135"
                    },
                    new ()
                    {
                        LaunchUrl = new Uri("https://api-test-v4.diaoq.com"),
                        ApiUrl = new Uri("https://spi-test.r4espt.com"),
                        BrandCode = "Default",
                        BrandCurrency = "HKD",
                        PublicToken = "a7b042594b234a72bdcc7d9bbc02537d",
                        PrivateToken = "000299ceeb83ecf381b01273de21f0ad0b0be634559cca779d95be195543c2f95204b14070c48248c59c28af564bbd391bd1bdf521260c9fc0dc816d5cd197e0dc0083734e57910dd7fc0148a5160ca571ebca344743d338e9b09219b08cb3104152f939fbf64f3c640897a45bed87dc7a0fe992b5ccd22d25fa0586544fcf632bf9",
                        PartnerId = "2145"
                    },
                    new ()
                    {
                        LaunchUrl = new Uri("https://api-test-v4.diaoq.com"),
                        ApiUrl = new Uri("https://spi-test.r4espt.com"),
                        BrandCode = "Default",
                        BrandCurrency = "IDR",
                        PublicToken = "4bb6f7d4cf6545fb81bba420c820f1e8",
                        PrivateToken = "00024a07b60ba4c16bc39d3b0bacfa3783f62dbd90518a28c95a1c9be0ced5ae231752b3e8cadd5c6cfd4bfd25d337b367a3ffb9309183c8719d047c4d197cf9ad5fc9c4bd6db9ffb3956823ee62ff8cd99bdbccfd605a4861ed5194dccf559c8dff51519601d275b584864360eb86fb092fca77efaad739092e96eef592d677e067",
                        PartnerId = "2140"
                    },
                    new ()
                    {
                        LaunchUrl = new Uri("https://api-test-v4.diaoq.com"),
                        ApiUrl = new Uri("https://spi-test.r4espt.com"),
                        BrandCode = "Default",
                        BrandCurrency = "IDR_1000",
                        PublicToken = "4d2df3a0012c41f5a1ee296cacb0dcb8",
                        PrivateToken = "00029affc6e40237819c4a0dc173217fd8af9849bb8d85b261bf4404b64a49f17ea3ddb49c71ccadc11a4b03c0df92027ffa520a91821d344b47ccef2df0ef1ed673c3ccb5be8b12c2ce81c8f3a9e8a11cfee30ac099f4f8024484f323260e5fa35590a04a4e09a4eb829790148cb8dad3b593811d4f7301e1f7b95cfd0c80687494",
                        PartnerId = "2141"
                    },
                    new ()
                    {
                        LaunchUrl = new Uri("https://api-test-v4.diaoq.com"),
                        ApiUrl = new Uri("https://spi-test.r4espt.com"),
                        BrandCode = "Default",
                        BrandCurrency = "INR",
                        PublicToken = "7a47508ed5e943608bdb455745ccac06",
                        PrivateToken = "0002bc3e32008f08c1c2bd8f3f962c4fb1956487e19cb502c14ebdc2606b7b80d9641c0f51b06661c115ee5863ac77c86548764eaf909c3a2fca71e6b3dfc66cec0b1cc8da109f4dc4de095d1d301d29e8b79eb5699c95170da734801bf3fbbaeb0c0bb1dfb9b1fc76bf1ebc26f0f03fd74e03f7f99aed6c4d70cc3297eac5dde0af",
                        PartnerId = "2144"
                    },
                    new ()
                    {
                        LaunchUrl = new Uri("https://api-test-v4.diaoq.com"),
                        ApiUrl = new Uri("https://spi-test.r4espt.com"),
                        BrandCode = "Default",
                        BrandCurrency = "JPY",
                        PublicToken = "71ce749382c2440eaa360573843ac4d8",
                        PrivateToken = "0002d0fa7cb1f3e705191af4b8e3849061c88a43c92a625676ca1f6335f8576a06a49f805db10ca638f805f0944a42d5ff462e1b62e3463d5c85bed44c5a8e09eff548ed1af839aec875b09299315d42e4101a075fad75c14c18afd1925656c0e8e82a34e694c9c1381ec542d6e814495b7812865c5756bf38f97d26a6525c1613fc",
                        PartnerId = "2142"
                    },
                    new ()
                    {
                        LaunchUrl = new Uri("https://api-test-v4.diaoq.com"),
                        ApiUrl = new Uri("https://spi-test.r4espt.com"),
                        BrandCode = "Default",
                        BrandCurrency = "KRW",
                        PublicToken = "bde0d1701584402385a5984827d4c29b",
                        PrivateToken = "00022dd713224e9d80b678f2643f526bd585a3682d276f04015a86d4451baa1d19515373ff07d2ddebdf45c441ddba1efeda742aa3da6abcd0f05d31aa6f19b7607756ac0ba5171c44e825c65cef471678aa8601ed622c26468577a92b35fe2849d0db722301c780e5829460f491519e9552b5e4b46c4ae8e2988397b14e488858b7",
                        PartnerId = "2143"
                    },
                    new ()
                    {
                        LaunchUrl = new Uri("https://api-test-v4.diaoq.com"),
                        ApiUrl = new Uri("https://spi-test.r4espt.com"),
                        BrandCode = "Default",
                        BrandCurrency = "MYR",
                        PublicToken = "25502c26dc0e4304abd197def3210429",
                        PrivateToken = "0002b201d434dd03033336378310e5c2a7a496d55c02b8bba16a50b36d71b626c5523155a7cb7022d0c22dc956ba6ff9ca3d36d5af1a1c5428d139a90618830d6b608733d60b92b63cbe0a0a112d33480bafde650566c9ad26f59e5af7bf4eb3309afcca2dc2eb9fd2f84d220feb16534e0bde7910beac969b105a4d3afa2de7ee42",
                        PartnerId = "2147"
                    },
                    new ()
                    {
                        LaunchUrl = new Uri("https://api-test-v4.diaoq.com"),
                        ApiUrl = new Uri("https://spi-test.r4espt.com"),
                        BrandCode = "Default",
                        BrandCurrency = "PHP",
                        PublicToken = "05801c39d71e4e69b38493418fe290ad",
                        PrivateToken = "0002b0c7fb727f2777a78bc26a96c85e10550041deedf043fa45082cb313b4e9a0544c68f5f804e958952633600e96ccbff3d54683022be40fffacf9694a4f69c6d89912236bc43af76bb11518b8a88aac32cbde73482f39a4dc3118738ed1d45ee11e5bb14d409a9090302dff5e05876a5d685ec949c763520488ddf9fa7be81c88",
                        PartnerId = "2146"
                    },
                    new ()
                    {
                        LaunchUrl = new Uri("https://api-test-v4.diaoq.com"),
                        ApiUrl = new Uri("https://spi-test.r4espt.com"),
                        BrandCode = "Default",
                        BrandCurrency = "THB",
                        PublicToken = "d215fa0f85884780b33e94c483bed2df",
                        PrivateToken = "0002db8f4d30242d18a0e96cac17209d0eb24e7a1cc415554ccfd929c1c3ca1c102c0869e8311a9d99084ebd18b7a23c0f6087b98071500d883c05a410e3551314dd27fac0a92d850b27fbf7b0e5bf529b44e50b9c000213ba0baa48f9f9577fed08fa91e958c28ee53a9b46ee034ed887916495b26b4e14ac5234bc7062be65a29f",
                        PartnerId = "2137"
                    },
                    new ()
                    {
                        LaunchUrl = new Uri("https://api-test-v4.diaoq.com"),
                        ApiUrl = new Uri("https://spi-test.r4espt.com"),
                        BrandCode = "Default",
                        BrandCurrency = "VND",
                        PublicToken = "50d7a43708c248c28526e7585f102ef8",
                        PrivateToken = "000272620ea5c386ec0c8ce1ee887ecc9961bd97e76ef81d92c3d5af486fa0116d85778b40488de5ae250f3aa4be07cd35151d3396b8cf9a706e889bd6259c0a3becdc39ffb94ef517247a6f0cdc28be479c9acbdc1df5d6fad45fb720aed3c85b2aa614dbbfa5949b48b1a225328790102e942e28bbf5d5539deb8fb2487e8e8636",
                        PartnerId = "2138"
                    },
                    new ()
                    {
                        LaunchUrl = new Uri("https://api-test-v4.diaoq.com"),
                        ApiUrl = new Uri("https://spi-test.r4espt.com"),
                        BrandCode = "Default",
                        BrandCurrency = "VND_1000",
                        PublicToken = "b0fd7672e6a7477c8d484d0a96053f83",
                        PrivateToken = "00025049ac289210cef5d307ed28248f2a48d80110122261e953d5656d018a58b3ff00f567d0f90fc203a8e67c638cb1939ee149291667ff268909eb53cfa329d7c48bcd06f8185ffeeb3b29bc06f35115d41824a964009b7baec22bca285efbdbca1a081d81ff85b5af597c1998a8604c8d35a5ef2ac6f97c452f07a9d189718b6d",
                        PartnerId = "2139"
                    }
                }
            };
        }

        internal class TFGameProviderConfiguration : IGameProviderConfiguration
        {
            string IGameProviderConfiguration.ClientId => "TFGaming-PG";

            string IGameProviderConfiguration.DesktopEndpoint => string.Empty;

            Guid IGameProviderConfiguration.Id => new("E3C3AD82-14D5-B345-AE1B-A9BF9E0C6D79");

            string IGameProviderConfiguration.MobileEndpoint => string.Empty;

            string IGameProviderConfiguration.Name => "Configuration TFGaming";

            string IGameProviderConfiguration.SecretKey => "4feq8YCryYHSGFW";
        }
    }
}
