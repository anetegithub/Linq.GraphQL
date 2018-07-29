namespace Linq.GraphQL.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    public static class ReflectionExtensions
    {
        internal static T New<T>(this Type type, ConstructorInfo ctor, params object[] argsObj)
        {
            ParameterInfo[] par = ctor.GetParameters();
            Expression[] args = new Expression[par.Length];
            ParameterExpression param = Expression.Parameter(typeof(object[]));
            for (int i = 0; i != par.Length; ++i)
            {
                args[i] = Expression.Convert(Expression.ArrayIndex(param, Expression.Constant(i)), par[i].ParameterType);
            }
            var expression = Expression.Lambda<Func<object[], T>>(
                Expression.New(ctor, args), param
            );

            var func = expression.Compile();

            return func(argsObj);
        }

        internal static object New(this Type type)
        {
            var ctor = type.GetConstructors().First();

            ParameterInfo[] par = ctor.GetParameters();
            Expression[] args = new Expression[par.Length];
            ParameterExpression param = Expression.Parameter(typeof(object[]));
            for (int i = 0; i != par.Length; ++i)
            {
                args[i] = Expression.Convert(Expression.ArrayIndex(param, Expression.Constant(i)), par[i].ParameterType);
            }
            var expression = Expression.Lambda<Func<object[], object>>(
                Expression.New(ctor, args), param
            );

            var func = expression.Compile();

            return func(new object[0]);
        }
    }
}