using Microsoft.AspNetCore.Mvc;

namespace TGS.Apis.GlobalApi.Models.Messages
{
    public abstract record PageableRequest
    {
        [BindProperty(Name = "page", SupportsGet = true)]
        public int Page { get; init; }

        [BindProperty(Name = "rows", SupportsGet = true)]
        public int Rows { get; init; }

        [BindProperty(Name = "showall", SupportsGet = true)]
        public bool ShowAll { get; init; } = true;
    }
}
