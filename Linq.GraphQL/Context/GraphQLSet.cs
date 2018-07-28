namespace Linq.GraphQL.Context
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Linq.GraphQL.Provider;

    public sealed class GraphQLSet<T> : GraphQLSet, IQueryable<T>
    {
        public GraphQLSet()
        {
            this.Expression = Expression.Constant(this);
        }

        public GraphQLSet(Expression expression)
        {
            this.Expression = expression;
        }

        public Type ElementType => typeof(T);

        public Expression Expression { get; private set; }

        public IQueryProvider Provider { get; } = new GraphQLProvider();

        public IEnumerator<T> GetEnumerator() => Provider.Execute<IQueryable<T>>(Expression).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    public abstract class GraphQLSet
    {
        public GraphQLSet<T> Generic<T>() => this as GraphQLSet<T>;
    }
}