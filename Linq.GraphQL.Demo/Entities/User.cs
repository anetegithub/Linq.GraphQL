namespace Linq.GraphQL.Demo.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class User : Identity
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public int Age { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Updated { get; set; }

        public IEnumerable<Thread> Threads { get; set; }

        public IEnumerable<Post> Posts { get; set; }
    }
}