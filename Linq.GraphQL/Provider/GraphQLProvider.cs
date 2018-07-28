namespace Linq.GraphQL.Provider
{
    using System.Linq;
    using System.Linq.Expressions;
    using Linq.GraphQL.Context;

    public class GraphQLProvider : IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            return this.CreateQuery<GraphQLSet>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new GraphQLSet<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return this.Execute<object>(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return default;
        }
    }
}
