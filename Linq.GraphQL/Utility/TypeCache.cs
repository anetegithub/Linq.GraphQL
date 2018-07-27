namespace Linq.GraphQL.Utility
{
    using System;
    using System.Collections.Concurrent;

    public sealed class TypeCache<T>
    {
        private ConcurrentDictionary<Type, object> container = new ConcurrentDictionary<Type, object>();

        public bool TryGet(Type type, out T cached)
        {
            cached = default;

            if (container.TryGetValue(type, out var value))
            {
                cached = (T)value;
                return true;
            }

            return false;
        }

        public bool TryAdd(Type type, T caching)
        {
            if (container.TryAdd(type, caching))
            {
                return true;
            }

            return false;
        }

    }
}