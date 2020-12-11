using System;
using System.Collections.Generic;
using System.Linq;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.DataInitializers
{
    public partial class DataInitializer
    {
        private static IEnumerable<TgsPlayer> GenerateTgsPlayers()
        {
            const int playerCountForPlayerType = 5;
            var playerTypes = new[] {PlayerType.RealPlayer, PlayerType.TestPlayer};
            var brands = Resellers.SelectMany(r => r.Licensees.SelectMany(l => l.Brands)).ToArray();

            return (from type in playerTypes
                    let prefix = type == PlayerType.RealPlayer ? "RP" : "TP"
                    from b in brands
                    from curr in b.Currencies
                    let currency = curr.Code.ToLower()
                    from lang in b.Languages
                    let language = lang.Code.ToLower()
                    from idx in Enumerable.Range(0, playerCountForPlayerType)
                    let name = $"{prefix}{idx + 1:00}_{currency}_{language}"
                    select new TgsPlayer(b.Id, name, name, language, currency, type)
                ).ToArray();
        }

        internal record TgsPlayer(Guid BrandId, string Username, string RealName, string CultureCode,
            string CurrencyCode, PlayerType Type);
    }
}