namespace Linq.GraphQL.Visitors
{
    using System;
    using System.Linq.Expressions;
    using FastMember;
    using Linq.GraphQL.Context;
    using Linq.GraphQL.QueryTree;

    public class GraphQLVisitor : ExpressionVisitor
    {
        private GraphQLContext graphQLContext;

        public GraphQLVisitor(GraphQLContext graphQLContext)
        {
            this.graphQLContext = graphQLContext;
        }

        private Entity current;
        protected Entity Current
        {
            get
            {
                if (current == null && QueryTree == null)
                {
                    QueryTree = new GraphQLQueryTree();
                    current = QueryTree;
                }

                return current;
            }

            set
            {
                if (QueryTree != null)
                {
                    QueryTree.Properties.Add(current = value);
                }
            }
        }

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

        protected override Expression VisitNew(NewExpression node)
        {
            if (QueryTree.TryFind(node.Type, out Entity entity))
            {
                BindProperties(entity);
            }

            Current = new Entity
            {
                CLRType = node.Type
            };

            BindProperties(Current);
            
            return base.VisitNew(node);
        }

        private void BindProperties(Entity entity)
        {
            foreach (var member in TypeAccessor.Create(entity.CLRType).GetMembers())
            {
                if (member.Type.IsPrimitive || member.Type.Equals(typeof(string)))
                {
                    entity.Properties.Add(new Property { Name = member.Name, CLRType = member.Type });
                }
                else
                {
                    entity.Properties.Add(new Entity
                    {
                        CLRType = member.Type
                    });
                }
            }
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            foreach (var member in graphQLContext.typeAccessor.GetMembers())
            {
                if (graphQLContext.typeAccessor[graphQLContext, member.Name] == node.Value)
                    Current.Name = member.Name;
            }

            return base.VisitConstant(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return base.VisitParameter(node);
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            return base.VisitMemberAssignment(node);
        }
    }
}
