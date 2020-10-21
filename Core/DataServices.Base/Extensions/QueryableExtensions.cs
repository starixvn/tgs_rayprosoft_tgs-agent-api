using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.Models.Messages;

namespace TGS.Core.DataServices.Base.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PagedResponse<TModel>> CreatePagedResponseAsync<TModel>(this IQueryable<TModel> query,
            PageableRequest request)
        {
            var result = query.OrderBy(request.GetSortOption());

            if (request.ShowAll)
            {
                var allResult = result.ToArray();
                return new PagedResponse<TModel>
                {
                    Valid = true,
                    TotalCount = allResult.Length,
                    Items = allResult
                };
            }

            var pagedResult = await result.ToPagedArrayAsync(request.Page, request.Rows);

            return new PagedResponse<TModel>
            {
                Valid = true,
                TotalCount = pagedResult.TotalCount,
                Items = pagedResult.Items
            };
        }
        public static Task<PagedResponse<TModel>> CreatePagedResponseAsync<TModel>(this IEnumerable<TModel> query,
            PageableRequest request)
        {
            var result = query.AsQueryable().OrderBy(request.GetSortOption());

            if (request.ShowAll)
            {
                var allResult = result.ToArray();
                return Task.FromResult(new PagedResponse<TModel>
                {
                    Valid = true,
                    TotalCount = allResult.Length,
                    Items = allResult
                });
            }

            var pagedResult = result.ToPagedArray(request.Page, request.Rows);

            return Task.FromResult(new PagedResponse<TModel>
            {
                Valid = true,
                TotalCount = pagedResult.TotalCount,
                Items = pagedResult.Items
            });
        }

        public static PagedArray<T> ToPagedArray<T>(this IQueryable<T> query, int page, int pageSize)
        {
            var totalCount = query.Count();
            page = page <= 0 ? 0 : page - 1;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            var items = query.Skip(page * pageSize).Take(pageSize).ToArray();
            return new PagedArray<T>(totalCount, items);
        }


        public static async Task<PagedArray<T>> ToPagedArrayAsync<T>(this IQueryable<T> query, int page, int pageSize)
        {
            var totalCount = await query.CountAsync();
            page = page <= 0 ? 0 : page - 1;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            var items = await query.Skip(page * pageSize).Take(pageSize).ToArrayAsync();
            return new PagedArray<T>(totalCount, items);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, SortOption sortOption)
        {
            if (!string.IsNullOrEmpty(sortOption.PropertyName))
            {
                return query.OrderByProperty(sortOption.PropertyName, sortOption.IsDescending);
            }
            return query;
        }

        public static IQueryable<TSource> OrderByProperty<TSource>(this IQueryable<TSource> source, string propertyName, bool descending)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TSource), "t");
            Expression orderByProperty = Expression.Property(parameter, propertyName);

            LambdaExpression lambda = Expression.Lambda(orderByProperty, new[] { parameter });

            MethodInfo genericMethod;

            if (@descending)
            {
                genericMethod = OrderByDescendingMethod.MakeGenericMethod(typeof(TSource), orderByProperty.Type);
            }
            else
            {
                genericMethod = OrderByMethod.MakeGenericMethod(typeof(TSource), orderByProperty.Type);
            }

            object result = genericMethod.Invoke(null, new object[] { source, lambda });

            return (IQueryable<TSource>)result;
        }

        private static readonly MethodInfo OrderByMethod =
            (from x in typeof(Queryable).GetMethods()
             where x.Name == "OrderBy"
             let pars = x.GetParameters()
             where pars.Length == 2
             select x).Single();

        private static readonly MethodInfo OrderByDescendingMethod =
            (from x in typeof(Queryable).GetMethods()
             where x.Name == "OrderByDescending"
             let pars = x.GetParameters()
             where pars.Length == 2
             select x).Single();


        // Where extension for filters of any nullable type
        public static IQueryable<TSource> Where<TSource, TFilter>(this IQueryable<TSource> source, TFilter? filter,
            Expression<Func<TSource, bool>> predicate) where TFilter : struct
        {
            if (filter.HasValue)
            {
                source = source.Where(predicate);
            }

            return source;
        }

        // Where extension for string filters
        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, string filter,
            Expression<Func<TSource, bool>> predicate)
        {
            if (!string.IsNullOrWhiteSpace(filter))
            {
                source = source.Where(predicate);
            }

            return source;
        }

        // Where extension for collection filters
        public static IQueryable<TSource> Where<TSource, TFilter>(this IQueryable<TSource> source, IEnumerable<TFilter> filter,
            Expression<Func<TSource, bool>> predicate)
        {
            if (filter != null && filter.Any())
            {
                source = source.Where(predicate);
            }

            return source;
        }

        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, bool isFiltered,
            Expression<Func<TSource, bool>> predicate)
        {
            if (isFiltered)
            {
                source = source.Where(predicate);
            }

            return source;
        }
    }
}
