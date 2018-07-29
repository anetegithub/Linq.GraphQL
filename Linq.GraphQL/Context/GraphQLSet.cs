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

        public IQueryProvider Provider { get; private set; }

        public IEnumerator<T> GetEnumerator() => Provider.Execute<IEnumerator<T>>(Expression);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        
        private string connectionString = string.Empty;
        internal override string ConnectionString
        {
            get => connectionString;

            set
            {
                this.connectionString = value;
                this.Provider = new GraphQLProvider(this.connectionString);
            }
        }
    }

    public abstract class GraphQLSet
    {
        public GraphQLSet<T> Generic<T>() => this as GraphQLSet<T>;

        internal abstract string ConnectionString { get; set; }
    }
}