using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Csizmazia.Collections
{
    internal static class OrderByExtensions
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
        {
            //applying OrderBy(property)
            return ApplyOrder(source, property, "OrderBy");
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
        {
            //applying OrderByDescending(property)
            return ApplyOrder(source, property, "OrderByDescending");
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string property)
        {
            //applying ThenBy(property)
            return ApplyOrder(source, property, "ThenBy");
        }

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string property)
        {
            //applying ThenByDescending(property)
            return ApplyOrder(source, property, "ThenByDescending");
        }

        private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            //apply Ordering(property,methodname)

            //creating order by expression statement
            string[] props = property.Split('.');
            Type type = typeof (T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ     
                PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }

            //creating orderby method type
            Type delegateType = typeof (Func<,>).MakeGenericType(typeof (T), type);

            //creating orderby labda statement
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            //invoking orderby methods
            object result = typeof (Queryable).GetMethods().Single(method => method.Name == methodName
                                                                             && method.IsGenericMethodDefinition
                                                                             && method.GetGenericArguments().Length == 2
                                                                             && method.GetParameters().Length == 2
                ).MakeGenericMethod(typeof (T), type)
                .Invoke(null, new object[]
                                  {
                                      source, lambda
                                  });

            return (IOrderedQueryable<T>) result;
        }
    }
}