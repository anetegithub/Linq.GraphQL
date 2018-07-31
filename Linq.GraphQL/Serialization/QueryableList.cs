namespace Linq.GraphQL.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class QueryableList<T> : List<T>, IQueryable<T>
    {
        public Type ElementType => typeof(T);

        public Expression Expression => Expression.Constant(this);

        public IQueryProvider Provider => null;
    }
}
