using JsonAutoService.ResourceHandlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace JsonAutoService.Swashbuckle
{
    class RelatableOperationId
    {
        [JsonProperty("resource_handler")]
        public string ResourceHandler { get; set; }

        [JsonProperty("procedure_name")]
        public string Procedure { get; set; }

        [JsonProperty("method_name")]
        public string Method { get; set; }

        [JsonProperty("http_method")]
        public string HttpMethod { get; set; }

        [JsonProperty("relative_path")]
        public string RelativePath { get; set; }

        public static Func<ApiDescription, string> Create()
        {
            return apiDesc =>
            {
                var endpointMetadata = apiDesc.ActionDescriptor.EndpointMetadata;

                foreach (object eMetadata in endpointMetadata)
                {
                    var result = Utility.TryCast<TypeFilterAttribute>(eMetadata, out TypeFilterAttribute typeFilterAttribute);

                    if (result)
                    {
                        if (typeFilterAttribute.ImplementationType.Name == nameof(JsonResourceResultsHandler) ||
                            typeFilterAttribute.ImplementationType.Name == nameof(JsonResourceContextHandler))
                        {
                            var relOperationId = new RelatableOperationId
                            {
                                ResourceHandler = typeFilterAttribute.ImplementationType.Name,
                                Method = apiDesc.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null,
                                Procedure = typeFilterAttribute.Arguments[0].ToString(),
                                HttpMethod = apiDesc.HttpMethod,
                                RelativePath = apiDesc.RelativePath
                            };

                            var json = JsonConvert.SerializeObject(relOperationId).ToString();
                            var base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
                            return base64String;
                        }
                    }
                }
                return null;
            };
        }
    }

    internal static class Utility
    {
        public static bool TryCast<T>(this object obj, out T result)
        {
            if (obj is T)
            {
                result = (T)obj;
                return true;
            }

            result = default(T);
            return false;
        }
    }
}
