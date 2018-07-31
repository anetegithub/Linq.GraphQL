namespace Linq.GraphQL.QueryTree
{
    using System.Collections.Generic;

    public class Entity : Property
    {
        public Filter Filter { get; set; }

        public ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}