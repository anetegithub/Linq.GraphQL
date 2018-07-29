namespace Linq.GraphQL.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Linq.GraphQL.QueryTree;

    public class GraphQLQueryTreeSerializer : IDisposable
    {
        private readonly GraphQLQueryTree entity;
        private StringBuilder result = new StringBuilder();
        private int level = 0;

        public GraphQLQueryTreeSerializer(GraphQLQueryTree entity)
        {
            this.entity = entity;
        }

        public string Serialize()
        {
            if (result.ToString() == "EOL")
                throw new ObjectDisposedException($"Current {nameof(GraphQLQueryTreeSerializer)} disposed!");

            this.SerializeEntity(this.entity);

            var serialized = this.result.ToString();

            this.result = new StringBuilder("EOL");

            return serialized;
        }

        private void SerializeEntity(Entity entity)
        {
            if (entity == null)
                return;

            if (!string.IsNullOrWhiteSpace(entity.Name))
            {
                result.Append(Indent + entity.Name);
            }

            if (entity.Filter != null)
            {
                this.SerializeFilter(entity.Filter);
            }

            if (entity.Properties != null && entity.Properties.Count > 0)
            {
                result.Append(" {");
                result.AppendLine();
                this.level++;
                this.SerializeProperties(entity.Properties);
                this.level--;
                result.Append(Indent + "}");
            }
        }

        private void SerializeFilter(Filter filter)
        {
            if (filter.Name == null || filter.Name.Count() == 0)
            {
                return;
            }

            if (filter.Value == null)
            {
                return;
            }

            var property = string.Empty;

            foreach (var prop in filter.Name)
            {
                property += prop.Name + Filter.Delimiter;
            }

            if (filter.Operation == Operation.equlas)
            {
                property = property.Substring(0, property.Length - Filter.Delimiter.Length);
            }
            else
            {
                property += filter.Operation.ToString();
            }

            string value = filter.Value.ToString();

            var isString = typeof(String).IsAssignableFrom(filter.Value.GetType());
            if (isString)
            {
                value = "\"" + value + "\"";
            }

            result.Append($"({property}:{value})");
        }

        private void SerializeProperties(IEnumerable<Property> properties)
        {
            var props = properties.ToList();
            foreach (var prop in props)
            {
                if (prop is Entity entityProp)
                {
                    this.SerializeEntity(entityProp);
                }
                else
                {
                    result.Append(Indent + prop.Name);
                }

                if (props.Last()!=prop)
                {
                    result.Append(",");
                }

                result.AppendLine();
            }
        }

        private string Indent => new String(Enumerable.Range(0, level).SelectMany(x => "  ").ToArray());

        public void Dispose()
        {
            result = new StringBuilder("EOL");
        }
    }
}