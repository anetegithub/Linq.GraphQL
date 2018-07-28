namespace Linq.GraphQL.Demo.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Thread : Identity
    {
        public string Title { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Updated { get; set; }

        public User User { get; set; }

        public IEnumerable<Post> Posts { get; set; }
    }
}