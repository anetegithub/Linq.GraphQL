namespace Linq.GraphQL.QueryTree
{
    using System.Collections.Generic;

    public class Filter
    {
        public LinkedList<Property> Name { get; set; }

        public Operation Operation { get; set; } = Operation.equlas;

        public object Value { get; set; }

        public const string Delimiter = "__";
    }
}