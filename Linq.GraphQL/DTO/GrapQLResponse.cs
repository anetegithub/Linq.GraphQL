namespace Linq.GraphQL.DTO
{
    public class GrapQLResponse<T>
    {
        public Metadata Meta { get; set; }

        public T[] Data { get; set; }
    }
}