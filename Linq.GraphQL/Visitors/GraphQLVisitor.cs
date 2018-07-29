namespace Linq.GraphQL.Visitors
{
    using System.Linq.Expressions;
    using Linq.GraphQL.QueryTree;

    public class GraphQLVisitor : ExpressionVisitor
    {
        protected GraphQLQueryTree QueryTree { get; set; }
        
        public new GraphQLQueryTree Visit(Expression node)
        {
            var result = base.Visit(node);
            return new GraphQLQueryTree();
        }
    }
}
