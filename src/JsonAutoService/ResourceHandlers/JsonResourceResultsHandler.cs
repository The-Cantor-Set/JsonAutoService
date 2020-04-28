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
    public class JsonResourceResultsHandler : Attribute, IAsyncResourceFilter
    {
        private readonly JsonAutoServiceOptions _options;
        private readonly IJsonAutoService _jsonAutoService;
        private readonly string procName;

        public JsonResourceResultsHandler(IOptionsMonitor<JsonAutoServiceOptions> options, IJsonAutoService jsonAutoService, object procName)
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
            var sqlConnection = _options.ConnectionString;

            //Handle request
            using (var reader = new StreamReader(request.Body))
            {
                var body = await reader.ReadToEndAsync();

                switch (request.Method)
                {
                    case nameof(SupportedMethods.GET):
                        var getResponse = await _jsonAutoService.SqlGetAsync(sqlConnection, jsonHeaders, procName, jsonRoutes);
                        context.Result = _jsonAutoService.JsonGetContentResult(getResponse.ToString());
                        break;
                    case nameof(SupportedMethods.PUT):
                        var putResponse = await _jsonAutoService.SqlPutAsync(sqlConnection, jsonHeaders, procName, jsonRoutes, body);
                        context.Result = _jsonAutoService.JsonPutContentResult(putResponse, _options.Mode, _options.ErrorThreshold);
                        break;
                    case nameof(SupportedMethods.POST):
                        var postResponse = await _jsonAutoService.SqlPostAsync(sqlConnection, jsonHeaders, procName, jsonRoutes, body);
                        context.Result = _jsonAutoService.JsonPostContentResult(postResponse, _options.Mode, _options.ErrorThreshold);
                        break;
                    case nameof(SupportedMethods.DELETE):
                        var deleteResponse = await _jsonAutoService.SqlDeleteAsync(sqlConnection, jsonHeaders, procName, jsonRoutes);
                        context.Result = _jsonAutoService.JsonDeleteContentResult(deleteResponse, _options.Mode, _options.ErrorThreshold);
                        break;
                    case nameof(SupportedMethods.HEAD):
                        var headResponse = await _jsonAutoService.SqlHeadAsync(sqlConnection, jsonHeaders, procName, jsonRoutes);
                        context.Result = _jsonAutoService.JsonHeadContentResult((bool)headResponse);
                        break;
                    default:
                        context.Result = _jsonAutoService.JsonDefaultContentResult();
                        break;
                }
            }
        }
    }
}
