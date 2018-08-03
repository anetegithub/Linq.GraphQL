namespace Linq.QueryPatcher
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class QueryPatcher
    {
        public static IQueryable<TSource> WithAccess<TSource>(this IQueryable<TSource> source, Func<TSource, bool> predicate)
        {
            Expression<Func<TSource, bool>> expr = x => predicate(x);

            return source.Provider.CreateQuery<TSource>(
                  Expression.Call(
                      null,
                      Where_TSource_2(typeof(TSource)),
                      source.Expression, Expression.Quote(expr)
                      ));

            return source.Provider.CreateQuery<TSource>(expr);
        }

        private static MethodInfo s_Where_TSource_2;

        public static MethodInfo Where_TSource_2(Type TSource) =>
             (s_Where_TSource_2 ??
             (s_Where_TSource_2 = new Func<IQueryable<object>, Expression<Func<object, bool>>, IQueryable<object>>(Queryable.Where).GetMethodInfo().GetGenericMethodDefinition()))
              .MakeGenericMethod(TSource);
    }
}