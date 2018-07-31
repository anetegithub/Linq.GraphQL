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
        private bool queryReport;
        private bool metaReport;

        public override object Materialize(GraphQLQueryTree queryTree, string connectionString, bool queryReport, bool metaReport)
        {
            this.queryReport = queryReport;
            this.metaReport = metaReport;
            return new GrapQLResponseEnumerator<T>(SendRequest(queryTree, connectionString));
        }

        private async Task<GraphQLResponse<T>> SendRequest(GraphQLQueryTree queryTree, string uri)
        {
            using (var client = new HttpClient())
            {
                using (var querySerializer = new GraphQLQueryTreeSerializer(queryTree))
                {
                    var query = querySerializer.Serialize();

                    if (queryReport)
                    {
                        System.Console.WriteLine("query:");
                        System.Console.WriteLine();
                        System.Console.WriteLine(query);
                        System.Console.WriteLine();
                        System.Console.WriteLine();
                    }

                    var content = new StringContent(query, Encoding.UTF8, "application/json");
                    var postTask = await client.PostAsync(uri, content);
                    if (postTask.IsSuccessStatusCode)
                    {
                        var data = await postTask.Content.ReadAsStringAsync();                        
                        var result = JsonConvert.DeserializeAnonymousType(data, new GraphQLResponse<T>(),new JsonSerializerSettings
                        {
                            ContractResolver = new JsonIQueryableContractResolver()
                        });
                        
                        if (metaReport)
                        {
                            System.Console.WriteLine("meta:");
                            System.Console.WriteLine();
                            System.Console.WriteLine(JsonConvert.SerializeObject(result.Meta));
                            System.Console.WriteLine();
                            System.Console.WriteLine();
                        }

                        return result;
                    }
                    else return default;
                }
            }
        }
    }

    public abstract class GraphQLQueryTreeMaterializer
    {
        public abstract object Materialize(GraphQLQueryTree queryTree, string connectionString, bool queryReport, bool metaReport);
    }
}