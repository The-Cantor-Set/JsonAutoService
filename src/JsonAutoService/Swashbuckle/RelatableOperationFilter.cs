using JsonAutoService.Service;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace JsonAutoService.Swashbuckle
{
    public class RelatableOperationFilter : IOperationFilter
    {
        private readonly IDictionary<string, string> _requiredHeaders;

        public RelatableOperationFilter(IOptions<JsonAutoServiceOptions> options)
        {
            _requiredHeaders = options.Value.RequiredHeaders;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            foreach (var item in _requiredHeaders)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = item.Key,
                    In = ParameterLocation.Header,
                    Description = item.Value,
                    Required = false,
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    }
                });
            }
        }
    }
}
