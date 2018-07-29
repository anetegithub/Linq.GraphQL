namespace Linq.GraphQL.Provider
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Linq.GraphQL.DTO;
    using Linq.GraphQL.QueryTree;
    using Linq.GraphQL.Serialization;
    using Newtonsoft.Json;

    public class GraphQLQueryTreeMaterializer<T> : GraphQLQueryTreeMaterializer
    {
        public override object Materialize(GraphQLQueryTree queryTree, string connectionString)
        {
            return new GrapQLResponseEnumerator<T>(SendRequest(queryTree, connectionString));
        }

        private async Task<GraphQLResponse<T>> SendRequest(GraphQLQueryTree queryTree, string uri)
        {
            using (var client = new HttpClient())
            {
                using (var querySerializer = new GraphQLQueryTreeSerializer(queryTree))
                {
                    var content = new StringContent(querySerializer.Serialize(), Encoding.UTF8, "application/json");
                    var postTask = await client.PostAsync(uri, content);
                    if (postTask.IsSuccessStatusCode)
                    {
                        var data = await postTask.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeAnonymousType(data, new GraphQLResponse<T>());
                        return result;
                    }
                    else return default;
                }
            }
        }
    }

    public abstract class GraphQLQueryTreeMaterializer
    {
        public abstract object Materialize(GraphQLQueryTree queryTree, string connectionString);
    }
}