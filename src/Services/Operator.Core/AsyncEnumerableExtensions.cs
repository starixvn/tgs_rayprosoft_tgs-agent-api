using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace TGS.Services.Operator
{
    public static class AsyncEnumerableExtensions
    {
        private static Type _AsyncEnumerableType = typeof(AsyncEnumerable);

        public static IOrderedAsyncEnumerable<T> OrderBy<T>(this IAsyncEnumerable<T> enumerable, LambdaExpression orderExpression)
        {
            var methodCallExp = Expression.Call(
                _AsyncEnumerableType
                    .GetGenericMethod(
                        nameof(AsyncEnumerable.OrderBy),
                        new[] { typeof(IAsyncEnumerable<>), typeof(Func<,>) })
                    !.MakeGenericMethod(new[] { typeof(T), orderExpression.ReturnType }),
                Expression.Constant(enumerable),
                Expression.Constant(orderExpression.Compile()));

            return Expression.Lambda<Func<IOrderedAsyncEnumerable<T>>>(
                methodCallExp).Compile()();
        }

        public static IOrderedAsyncEnumerable<T> OrderByDescending<T>(this IAsyncEnumerable<T> enumerable, LambdaExpression orderExpression)
        {
            var methodCallExp = Expression.Call(
               _AsyncEnumerableType
                   .GetGenericMethod(
                       nameof(AsyncEnumerable.OrderByDescending),
                       new[] { typeof(IAsyncEnumerable<>), typeof(Func<,>) })
                   !.MakeGenericMethod(new[] { typeof(T), orderExpression.ReturnType }),
               Expression.Constant(enumerable),
               Expression.Constant(orderExpression.Compile()));

            return Expression.Lambda<Func<IOrderedAsyncEnumerable<T>>>(
                methodCallExp).Compile()();
        }
    }
}
