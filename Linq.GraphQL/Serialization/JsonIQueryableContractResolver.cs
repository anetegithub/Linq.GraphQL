namespace Linq.GraphQL.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json.Serialization;

    public class JsonIQueryableContractResolver : DefaultContractResolver
    {
        protected override JsonContract CreateContract(Type objectType)
        {
            if(typeof(IQueryable).IsAssignableFrom(objectType))
            {
                return base.CreateArrayContract(typeof(QueryableList<>).MakeGenericType(objectType.GetGenericArguments().FirstOrDefault()));
            }

            return base.CreateContract(objectType);
        }
    }
}
