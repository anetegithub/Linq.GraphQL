namespace Linq.GraphQL.Visitors
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FastMember;
    using Linq.GraphQL.Context;
    using Linq.GraphQL.QueryTree;

    public class GraphQLVisitor : ExpressionVisitor
    {
        private GraphQLContext graphQLContext;
        private readonly Dictionary<ExpressionType, Operation> OperationBind = new Dictionary<ExpressionType, Operation>()
        {
            { ExpressionType.Equal, Operation.equlas },
            { ExpressionType.NotEqual, Operation.not },
            { ExpressionType.LessThan, Operation.lt},
            { ExpressionType.LessThanOrEqual, Operation.equlas },
            { ExpressionType.GreaterThan, Operation.gt },
            { ExpressionType.GreaterThanOrEqual, Operation.gte }
        };

        private readonly Dictionary<string, Operation> MethodBind = new Dictionary<string, Operation>()
        {
            { "contains", Operation.contains},
            { "startswith", Operation.startswith},
            { "endswith", Operation.endswith}
        };

        public GraphQLVisitor(GraphQLContext graphQLContext)
        {
            this.graphQLContext = graphQLContext;            
        }

        private bool rootProcessed = false;

        protected GraphQLQueryTree QueryTree { get; set; } = new GraphQLQueryTree();
        
        public new GraphQLQueryTree Visit(Expression node)
        {
            var result = base.Visit(node);
            return QueryTree;
        }
        
        protected override Expression VisitBinary(BinaryExpression node)
        {
            Expression @base() => base.VisitBinary(node);

            if (!TryGetOperation(node, out var operation))
                return @base();

            if (!TryGetEntity(node.Left, out var entity))
                return @base();

            if (!TryGetProperty(node.Left, out var property))
                return @base();

            if (!TryGetValue(node.Right, out var value))
                return @base();

            entity.Filter = new Filter
            {
                Name=property,
                Operation=operation,
                Value=value
            };

            return @base();
        }

        private bool TryGetOperation(BinaryExpression node, out Operation outOpertation)
        {
            outOpertation = Operation.equlas;

            if (OperationBind.TryGetValue(node.NodeType, out Operation operation))
            {
                outOpertation = operation;
                return true;
            }

            return false;
        }

        private bool TryGetProperty(Expression leftExpression, out LinkedList<Property> propertyName)
        {
            propertyName = new LinkedList<Property>();

            if (!(leftExpression is MemberExpression memberExpression))
                return false;

            if (!(QueryTree.TryFindPath(memberExpression.Member.ReflectedType, out var path)))
                return false;

            path.AddLast(new Property { Name = memberExpression.Member.Name });

            propertyName = path;

            return true;
        }

        private bool TryGetEntity(Expression leftExpression, out Entity entity)
        {
            entity = null;

            if (!(leftExpression is MemberExpression memberExpression))
                return false;

            if (!(QueryTree.TryFind(memberExpression.Member.ReflectedType, out var findedEntity)))
                return false;

            entity = findedEntity;

            return true;
        }

        private bool TryGetValue(Expression rightExpression, out object value)
        {
            value = null;

            if (!(rightExpression is ConstantExpression rightConstant))
                return false;

            value = rightConstant.Value;
            return true;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            Expression @base() => base.VisitMethodCall(node);

            if (!TryGetOperationMethod(node, out var operation))
                return @base();

            if (!TryGetEntity(node.Object, out var entity))
                return @base();

            if (!TryGetProperty(node.Object, out var property))
                return @base();

            if (!TryGetValue(node.Arguments.FirstOrDefault(), out var value))
                return @base();
            
            entity.Filter = new Filter
            {
                Name = property,
                Operation = operation,
                Value = value
            };

            return @base();
        }
        
        private bool TryGetOperationMethod(MethodCallExpression node, out Operation outOpertation)
        {
            outOpertation = Operation.equlas;

            if (MethodBind.TryGetValue(node.Method.Name.ToLowerInvariant(), out Operation operation))
            {
                outOpertation = operation;
                return true;
            }

            return false;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            if (!rootProcessed)
            {
                QueryTree.CLRType = node.Type;
                BindProperties(QueryTree);
                rootProcessed = true;
            }

            return base.VisitNew(node);
        }

        private void BindProperties(Entity entity)
        {
            foreach (var member in TypeAccessor.Create(entity.CLRType).GetMembers())
            {
                var memberType = member.Type;
                if (memberType.IsPrimitive || memberType.Equals(typeof(string)) || Nullable.GetUnderlyingType(memberType) != null)
                {
                    entity.Properties.Add(new Property { Name = member.Name.ToLowerInvariant(), CLRType = member.Type });
                }
                else
                {
                    var CLRType = member.Type;

                    if (typeof(IEnumerable).IsAssignableFrom(member.Type))
                    {
                        CLRType = member.Type.GetGenericArguments()[0];
                    }

                    var newEntityNode = new Entity
                    {
                        Name = member.Name.ToLowerInvariant(),
                        CLRType = CLRType
                    };
                    entity.Properties.Add(newEntityNode);
                    BindProperties(newEntityNode);
                }
            }
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            // здесь получаем первое имя того что выбираем
            foreach (var member in graphQLContext.typeAccessor.GetMembers())
            {
                if (graphQLContext.typeAccessor[graphQLContext, member.Name] == node.Value)
                {
                    if (!rootProcessed)
                        QueryTree.Name = member.Name.ToLowerInvariant();
                }
            }

            return base.VisitConstant(node);
        }
    }
}
