namespace Linq.GraphQL.QueryTree
{
    using System;
    using System.Collections.Generic;

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

            finded = Descendant(CLRType, this);

            return finded != null;
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

        public bool TryFindPath(Type CLRType, out LinkedList<Property> finded)
        {
            finded = new LinkedList<Property>();

            if (this.CLRType == null)
                return false;

            finded.AddLast(this);
            if (this.CLRType == CLRType)
                return true;

            DescendantPath(CLRType, this, finded);

            return finded.Count > 0;
        }

        private void DescendantPath(Type CLRType, Entity entity, LinkedList<Property> context)
        {
            foreach (var item in entity.Properties)
            {
                if (!(item is Entity entityItem))
                    continue;

                context.AddLast(item);

                if (entityItem.CLRType == CLRType)
                    return;

                DescendantPath(CLRType, item as Entity, context);
            }
        }
    }
}