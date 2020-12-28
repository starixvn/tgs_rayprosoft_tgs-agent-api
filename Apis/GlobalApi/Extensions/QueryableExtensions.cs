using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.GlobalApi.Models.Messages;

namespace TGS.Apis.GlobalApi.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PagedResponse<TModel>> CreatePagedResponseAsync<TResponse, TModel>(
            this IQueryable<TModel> query,
            PageableRequest request) where TResponse : PagedResponse<TModel> where TModel : class
        {
            if (request.ShowAll || request.Rows == 0)
            {
                var allResult = await query.ToArrayAsync();
                return new()
                {
                    TotalCount = allResult.Length,
                    Items = allResult
                };
            }

            var pagedResult = await query.ToPagedArrayAsync(request.Page, request.Rows);

            return new()
            {
                TotalCount = pagedResult.TotalCount,
                Items = pagedResult.Items
            };
        }

        public static async Task<PagedArray<T>> ToPagedArrayAsync<T>(this IQueryable<T> query, int page, int pageSize)
            where T : class
        {
            var totalCount = await query.CountAsync();
            page = page <= 0 ? 0 : page - 1;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            var items = await query.Skip(page * pageSize).Take(pageSize).ToArrayAsync();
            return new(totalCount, items);
        }
    }
}