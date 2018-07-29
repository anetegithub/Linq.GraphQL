namespace Linq.GraphQL.Provider
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using FastMember;
    using Linq.GraphQL.Context;
    using Linq.GraphQL.Extensions;
    using Linq.GraphQL.QueryTree;
    using Linq.GraphQL.Serialization;
    using Linq.GraphQL.Visitors;
    using Newtonsoft.Json;

    public class GraphQLProvider : IQueryProvider
    {
        private readonly string uri;

        public GraphQLProvider(string connectionString)
        {
            this.uri = connectionString;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return this.CreateQuery<GraphQLSet>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new GraphQLSet<TElement>(expression)
            {
                ConnectionString = uri
            };
        }

        public object Execute(Expression expression)
        {
            return this.Execute<object>(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            if (!IsSupported<TResult>())
                return default;

            var visitor = new GraphQLVisitor();

            var query = visitor.Visit(expression);

            var materializeType = typeof(GraphQLQueryTreeMaterializer<>).MakeGenericType(this.DataType<TResult>());

            var materializer = (GraphQLQueryTreeMaterializer)materializeType.New();

            return (TResult)materializer.Materialize(query, uri);
        }
        
        private bool IsSupported<T>() => typeof(IEnumerator).IsAssignableFrom(typeof(T));
        private Type DataType<T>() => typeof(T).GetGenericArguments().First();
    }
}