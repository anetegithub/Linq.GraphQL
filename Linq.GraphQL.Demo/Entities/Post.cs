namespace Linq.GraphQL.Demo.Entities
{
    using System;

    public class Post : Identity
    {
        public string Body { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Updated { get; set; }

        public User User { get; set; }

        public Thread Thread { get; set; }
    }
}