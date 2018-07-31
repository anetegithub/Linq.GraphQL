namespace Linq.GraphQL.Context
{
    using System;
    using FastMember;
    using Linq.GraphQL.Extensions;
    using Linq.GraphQL.Utility;

    public abstract class GraphQLContext : IDisposable
    {
        internal readonly TypeAccessor typeAccessor;

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
                    var setProperty = typeAccessor[this, member.Name];
                    setProperty = TypeAccessor.Create(member.Type).CreateNew();

                    var grapQLSetProperty = setProperty as GraphQLSet;
                    grapQLSetProperty.GraphQLContext = this;
                    grapQLSetProperty.ConnectionString = connectionString;

                    typeAccessor[this, member.Name] = setProperty;
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