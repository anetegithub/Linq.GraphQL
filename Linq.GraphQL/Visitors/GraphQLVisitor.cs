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
           
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return base.VisitLambda(node);
        }
        
        protected override Expression VisitMember(MemberExpression node)
        {
            return base.VisitMember(node);
        }

        protected override MemberBinding VisitMemberBinding(MemberBinding node)
        {
            return base.VisitMemberBinding(node);
        }


        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            return base.VisitMemberInit(node);
        }

        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            return base.VisitMemberMemberBinding(node);
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            return base.VisitMemberAssignment(node);
        }
        
        protected override Expression VisitNew(NewExpression node)
        {
            return base.VisitNew(node);
        }
    }
}
