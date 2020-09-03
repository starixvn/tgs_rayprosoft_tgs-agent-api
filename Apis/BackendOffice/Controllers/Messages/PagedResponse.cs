namespace TGS.Apis.BackendOffice.Controllers.Messages
{
    public class PagedResponse<T> : MessageResponse
    {
        public PagedResponse()
        {
        }

        public PagedResponse(int count, T[] items)
        {
            TotalCount = count;
            Items = items;
        }

        public int TotalCount { get; set; }
        public T[] Items { get; set; }
    }
}
