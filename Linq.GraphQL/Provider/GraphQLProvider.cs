namespace Linq.GraphQL.Provider
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Linq.Expressions;
    using Linq.GraphQL.Context;
    using Linq.GraphQL.Extensions;
    using Linq.GraphQL.Visitors;

    public class GraphQLProvider : IQueryProvider
    {
        private readonly string uri;
        private readonly bool QueryReport;
        private readonly bool MetaReport;
        private GraphQLContext graphQLContext;

        public GraphQLProvider(string connectionString, GraphQLContext graphQLContext, bool QueryReport, bool MetaReport)
        {
            this.uri = connectionString;
            this.graphQLContext = graphQLContext;
            this.QueryReport = QueryReport;
            this.MetaReport = MetaReport;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return this.CreateQuery<GraphQLSet>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new GraphQLSet<TElement>(expression)
            {
                GraphQLContext = graphQLContext,
                QueryReport = QueryReport,
                MetaReport = MetaReport,
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

            var visitor = new GraphQLVisitor(this.graphQLContext);

            var query = visitor.Visit(expression);

            var materializeType = typeof(GraphQLQueryTreeMaterializer<>).MakeGenericType(this.DataType<TResult>());

            var materializer = (GraphQLQueryTreeMaterializer)materializeType.New();

            return (TResult)materializer.Materialize(query, uri, this.QueryReport, this.MetaReport);
        }
        
        private bool IsSupported<T>() => typeof(IEnumerator).IsAssignableFrom(typeof(T));

        private Type DataType<T>() => typeof(T).GetGenericArguments().First();
    }
}