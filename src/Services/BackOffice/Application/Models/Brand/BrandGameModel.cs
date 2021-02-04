using System;

namespace TGS.Services.BackOffice.Application.Models.Brand
{
    public class BrandGameModel
    {
        public Guid GameProviderId { get; set; }
        public int GameId { get; set; }
        public string GameName { get; set; }
    }
}