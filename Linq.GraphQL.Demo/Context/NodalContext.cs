namespace Linq.GraphQL.Demo
{
    using Linq.GraphQL.Context;
    using Linq.GraphQL.Demo.Entities;

    public class NodalContext : GraphQLContext
    {
        public GraphQLSet<User> Users { get; set; }

        public GraphQLSet<Thread> Threads { get; set; }

        public GraphQLSet<Post> Posts { get; set; }
    }
}