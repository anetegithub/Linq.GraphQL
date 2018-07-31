using System;
using System.Collections.Generic;
using System.Linq;
using Linq.GraphQL.Context;
using Linq.GraphQL.Demo.Entities;
using Linq.GraphQL.QueryTree;
using Linq.GraphQL.Serialization;
using Newtonsoft.Json;

namespace Linq.GraphQL.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            //threads {
            //    id,
            //  title,
            //  user {
            //        id,
            //    username
            //  },
            //  posts(body__contains: "hello") {
            //        body,
            //    user {
            //            username
            //    },
            //    created_at
            //  }
            //}

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
                    posts = ctx.Posts.Where(p => p.Body.Contains("hello")).Select(p => new
                    {
                        p.Body,
                        user = p.User.UserName,
                        p.Created
                    })
                }).Where(x => x.Id > 1);
                
                foreach (var item in data)
                {
                    var json = JsonConvert.SerializeObject(item);
                    Console.WriteLine(json);
                }
            }

            Console.WriteLine("end");
            Console.ReadLine();
        }

        static GraphQLQueryTree Query
        {
            get
            {
                return new GraphQLQueryTree
                {
                    Name = "threads",
                    Properties = new Property[]
                    {
                        new Property { Name="id"},
                        new Property { Name="title"},
                        new Entity {
                            Name ="user",
                            Properties=new Property[]
                            {
                                new Property{Name="id"},
                                new Property{Name="username"}
                            }
                        },
                        new Entity
                        {
                            Name="posts",
                            Filter=new Filter
                            {
                                Name=new LinkedList<Property>(new Property[]{new Property { Name="body"} }),
                                Operation= Operation.contains,
                                 Value="hello"
                            },
                            Properties=new Property[]
                            {
                                new Property{ Name="body"},
                                new Entity
                                {
                                    Name="user",
                                    Properties=new Property[]
                                    {
                                        new Property{Name="username"}
                                    }
                                },
                                new Property{Name="created_at"}
                            }
                        }
                    }
                };
            }
        }
    }
}