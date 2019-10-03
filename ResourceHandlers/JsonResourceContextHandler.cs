using JsonAutoService.Service;
using JsonAutoService.Structures;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace JsonAutoService.ResourceHandlers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class JsonResourceContextHandler : Attribute, IAsyncResourceFilter
    {
        private readonly JsonAutoServiceOptions _options;
        private readonly IJsonAutoService _jsonAutoService;
        private readonly string procName;

        public JsonResourceContextHandler(IOptionsMonitor<JsonAutoServiceOptions> options, IJsonAutoService jsonAutoService, object procName)
        {
            this._options = options.CurrentValue;
            this._jsonAutoService = jsonAutoService;
            this.procName = procName.ToString();
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            var request = httpContext.Request;
            var user = httpContext.User;

            //Serialize Routes
            var jsonRoutes = JsonConvert.SerializeObject(context.RouteData.Values);

            //Serialize Headers
            var headerDictionary = _jsonAutoService.GetHeadersDictionary(request.Headers, user);
            var jsonHeaders = JsonConvert.SerializeObject(headerDictionary);

            //Get connection string
            var sqlConnection = httpContext.Items[_options.ConnectionStringName].ToString();

            //Handle request
            using (var reader = new StreamReader(request.Body))
            {
                var body = await reader.ReadToEndAsync();

                switch (request.Method)
                {
                    case nameof(SupportedMethods.GET):
                        httpContext.Items[nameof(GetResult)] = new GetResult(await _jsonAutoService.SqlGetAsync(sqlConnection, jsonHeaders, procName, jsonRoutes));
                        break;
                    case nameof(SupportedMethods.PUT):
                        httpContext.Items[nameof(PutResult)] = await _jsonAutoService.SqlPutAsync(sqlConnection, jsonHeaders, procName, jsonRoutes, body);
                        break;
                    case nameof(SupportedMethods.POST):
                        httpContext.Items[nameof(PostResult)] = await _jsonAutoService.SqlPostAsync(sqlConnection, jsonHeaders, procName, jsonRoutes, body);
                        break;
                    case nameof(SupportedMethods.DELETE):
                        httpContext.Items[nameof(DeleteResult)] = await _jsonAutoService.SqlDeleteAsync(sqlConnection, jsonHeaders, procName, jsonRoutes);
                        break;
                    case nameof(SupportedMethods.HEAD):
                        httpContext.Items[nameof(HeadResult)] = new HeadResult(await _jsonAutoService.SqlHeadAsync(sqlConnection, jsonHeaders, procName, jsonRoutes));
                        break;
                    default:
                        context.Result = _jsonAutoService.JsonDefaultContentResult();
                        break;
                }
                await next();
            }
        }
    }
}
