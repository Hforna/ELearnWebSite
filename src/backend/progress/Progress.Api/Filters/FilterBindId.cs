using Microsoft.OpenApi.Models;
using Progress.Api.Binders;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Progress.Api.Filters
{
    public class FilterBindId : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var encryptedIds = context.ApiDescription.ParameterDescriptions
            .Where(x => x.ModelMetadata != null && x.ModelMetadata.BinderType == typeof(BinderId))
            .ToDictionary(d => d.Name, d => d);

            foreach (var parameter in operation.Parameters)
            {
                if (encryptedIds.TryGetValue(parameter.Name, out var apiParameter))
                {
                    parameter.Schema.Format = string.Empty;
                    parameter.Schema.Type = "string";
                }
            }

            foreach (var schema in context.SchemaRepository.Schemas.Values)
            {
                foreach (var property in schema.Properties)
                {
                    if (encryptedIds.TryGetValue(property.Key, out var apiParameter))
                    {
                        property.Value.Format = string.Empty;
                        property.Value.Type = "string";
                    }
                }
            }
        }
    }
}
