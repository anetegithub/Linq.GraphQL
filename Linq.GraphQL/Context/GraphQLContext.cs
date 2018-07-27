namespace Linq.GraphQL.Context
{
    using Linq.GraphQL.Utility;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public abstract class GraphQLContext
    {
        private readonly TypeCache<GraphQLSet> SetCache = new TypeCache<GraphQLSet>();

        public GraphQLSet<T> EntitySet<T>()
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
    }
}