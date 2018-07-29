namespace Linq.GraphQL.Context
{
    using System;
    using FastMember;
    using Linq.GraphQL.Extensions;
    using Linq.GraphQL.Utility;

    public abstract class GraphQLContext : IDisposable
    {
        private readonly TypeAccessor typeAccessor;

        private readonly string connectionString;
        public GraphQLContext(string connectionString)
        {
            this.connectionString = connectionString;
            typeAccessor = TypeAccessor.Create(this.GetType(), true);
            this.BindConnectionString();
        }

        private void BindConnectionString()
        {
            foreach (var member in typeAccessor.GetMembers())
            {
                if (typeof(GraphQLSet).IsAssignableFrom(member.Type))
                {
                    typeAccessor[this, member.Name] = TypeAccessor.Create(member.Type).CreateNew();
                    if (typeAccessor[this, member.Name] is GraphQLSet memberSet)
                    {
                        memberSet.ConnectionString = connectionString;
                    }
                }
            }
        }

        private readonly TypeCache<GraphQLSet> SetCache = new TypeCache<GraphQLSet>();

        public GraphQLSet<T> Set<T>()
        {
            var genericSetType = typeof(GraphQLSet<T>);
            if (SetCache.TryGet(genericSetType, out var set))
            {
                return set.Generic<T>();
            }

            var genericSet = new GraphQLSet<T>();
            if (SetCache.TryAdd(genericSetType, genericSet))
            {
                return genericSet;
            }

            throw new Exception($"Невозможн получить и создать источник сущности {typeof(T)}");
        }

        public void Dispose() { }
    }
}