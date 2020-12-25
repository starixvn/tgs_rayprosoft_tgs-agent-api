namespace TGS.Apis.GlobalApi.Models.Messages
{
    public record PagedArray<T>(int TotalCount, T[] Items) where T : class;
}
