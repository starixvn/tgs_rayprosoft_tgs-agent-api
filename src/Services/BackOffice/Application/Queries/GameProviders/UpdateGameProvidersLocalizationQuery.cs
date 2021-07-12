using System.Collections.Generic;

namespace TGS.Services.BackOffice.Application.Queries.GameProviders
{
    public class UpdateGameProvidersLocalizationQuery
    {
        public List<GameProvidersLocalizationModel> GameProviders { get; set; }
        public string EditorUsername { get; set; }
    }
}