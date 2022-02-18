namespace TGS.Services.Agent
{
    public record PagingInfo(int Page, int RowCount, string? SortBy, bool IsDesc);
}
