namespace Linq.GraphQL.Demo
{
    using Newtonsoft.Json;
    using System;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            using (var ctx = new NodalContext("http://graphql.nodaljs.com/graph"))
            {
                var data = ctx.Threads.Select(t => new
                {
                    t.Id,
                    t.Title,
                    user = new
                    {
                        t.User.Id,
                        t.User.UserName
                    },
                    posts = ctx.Posts.Select(p => new
                    {
                        p.Body,
                        user = new { p.User.UserName },
                        p.Created
                    }).Where(p => !p.Body.Contains("hello"))
                }).Where(x => x.Id > 1);

                Console.WriteLine("sync?");
                bool typed = false;

                foreach (var item in data)
                {
                    if (!typed)
                    {
                        typed = true;
                        Console.WriteLine("Typed result:");
                    }

                    var json = JsonConvert.SerializeObject(item);
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine(json);
                    Console.WriteLine();
                }
            }

            Console.WriteLine("end");
            Console.ReadLine();
        }
    }
}