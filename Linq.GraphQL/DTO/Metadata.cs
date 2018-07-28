namespace Linq.GraphQL.DTO
{
    public sealed class Metadata
    {
        public int Total { get; set; }
        public int Count { get; set; }
        public int Offset { get; set; }
        public Error Error { get; set; }
    }
}