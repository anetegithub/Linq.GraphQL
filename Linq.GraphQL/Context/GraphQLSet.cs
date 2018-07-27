namespace Linq.GraphQL.Context
{
    using Bars.Linq.Async;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public sealed class GraphQLSet<T> : GraphQLSet, IAsyncQueryable<T>
    {
        public Type ElementType => this.GetType();

        public Expression Expression { get; private set; }

        public IQueryProvider Provider { get; private set; }

        public IEnumerator<T> GetEnumerator() => Provider.Execute<IQueryable<T>>(Expression).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public IAsyncQueryProvider<T> AsyncProvider { get; private set; }

        public IAsyncEnumerator<T> GetAsyncEnumerator() => AsyncProvider.AsyncExecute(Expression).GetAsyncEnumerator();
    }

    public abstract class GraphQLSet
    {
        public GraphQLSet<T> Generic<T>() => this as GraphQLSet<T>;
    }
}