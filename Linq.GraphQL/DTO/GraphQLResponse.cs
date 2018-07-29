namespace Linq.GraphQL.DTO
{
    public class GraphQLResponse<T>
    {
        public Metadata Meta { get; set; }

        public T[] Data { get; set; }
    }
}