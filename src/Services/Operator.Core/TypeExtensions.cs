using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TGS.Services.Operator
{
    public static class TypeExtensions
    {
        public static LambdaExpression MakeMemberAccess(this Type type, string propertyName)
        {
            var propertyInfo = type.GetProperty(propertyName)!;
            var parameter = Expression.Parameter(type, "o");
            var propertyAccess = Expression.MakeMemberAccess(parameter, propertyInfo);

            return Expression.Lambda(propertyAccess, parameter);
        }

        public static MethodInfo? GetGenericMethod(this Type type, string methodName, Type[] parameterTypes)
        {
            return type.GetMethods()
                .Where(mi => mi.Name == nameof(AsyncEnumerable.OrderByDescending))
                .Where(mi => parameterTypes.SequenceEqual(mi.GetParameters().Select(p => p.ParameterType), TypeEqualityComparer.Default))
                .FirstOrDefault();
        }

        private class TypeEqualityComparer : IEqualityComparer<Type>
        {
            public static TypeEqualityComparer Default => new TypeEqualityComparer();

            public bool Equals(Type? x, Type? y)
            {
                return x?.Assembly == y?.Assembly
                    && x?.Namespace == y?.Namespace
                    && x?.Name == y?.Name;
            }

            public int GetHashCode([DisallowNull] Type obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}
