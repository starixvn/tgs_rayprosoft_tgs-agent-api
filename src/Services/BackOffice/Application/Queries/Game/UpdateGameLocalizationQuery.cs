using System.Collections.Generic;

namespace TGS.Services.BackOffice.Application.Queries.Game
{
    public class UpdateGameLocalizationQuery
    {
        public List<GameLocalizationModel> Games { get; set; }
        public string EditorUsername { get; set; }
    }
}