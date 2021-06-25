namespace TGS.Services.Operator
{
    public record Tag
    {
        public bool New { get; init; }

        public bool Hot { get; init; }

        public bool Recommend { get; init; }
    }
}