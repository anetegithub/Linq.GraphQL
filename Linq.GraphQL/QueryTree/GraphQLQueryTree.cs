namespace Linq.GraphQL.QueryTree
{
    using System;

    public class GraphQLQueryTree : Entity
    {
        public bool TryFind(Type CLRType, out Entity finded)
        {
            finded = default;
            if (this.CLRType == null)
                return false;

            if (this.CLRType == CLRType)
            {
                finded = this;
                return true;
            }

            finded = Descendant(CLRType, finded);

            return finded == null;
        }

        private Entity Descendant(Type CLRType, Entity entity)
        {
            foreach (var item in entity.Properties)
            {
                var entityItem = item as Entity;

                if (entityItem == null)
                    continue;


                if (entityItem.CLRType == CLRType)
                    return entityItem;

                var descendantFind = Descendant(CLRType, item as Entity);
                if (descendantFind != null)
                    return descendantFind;
            }

            return default;
        }
    }
}