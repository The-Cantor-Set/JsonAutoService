using JsonAutoService.Service;
using JsonAutoService.Structures;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace JsonAutoService.ResourceHandlers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class JsonResourceResultsHandler : Attribute, IAsyncResourceFilter
    {
        private readonly JsonAutoServiceOptions _options;
        private readonly IJsonAutoService _jsonAutoService;
        private readonly string _procName;
        private ILogger _logger;

        public JsonResourceResultsHandler(IOptionsMonitor<JsonAutoServiceOptions> options, IJsonAutoService jsonAutoService, object procName,ILoggerFactory loggerFactory)
        {
            _options = options.CurrentValue;
            _jsonAutoService = jsonAutoService;
            _procName = procName.ToString();
            _logger = loggerFactory.CreateLogger<JsonResourceResultsHandler>();
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            _logger.LogInformation("Entered OnResourceExecutionAsync() method");
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
                        _logger.LogInformation($"Executing GET request for {_procName} in db {sqlConnection}");
                        var getResponse = await _jsonAutoService.SqlGetAsync(sqlConnection, jsonHeaders, _procName, jsonRoutes);
                        context.Result = _jsonAutoService.JsonGetContentResult((string)getResponse);
                        _logger.LogInformation("Result retrieved");
                        break;
                    case nameof(SupportedMethods.PUT):
                        _logger.LogInformation($"Executing PUT request for {_procName} in db {sqlConnection}");
                        var putResponse = await _jsonAutoService.SqlPutAsync(sqlConnection, jsonHeaders, _procName, jsonRoutes, body);
                        context.Result = _jsonAutoService.JsonPutContentResult(putResponse, _options.Mode, _options.ErrorThreshold);
                        _logger.LogInformation("Result retrieved");
                        break;
                    case nameof(SupportedMethods.POST):
                        _logger.LogInformation($"Executing POST request for {_procName} in db {sqlConnection}");
                        var postResponse = await _jsonAutoService.SqlPostAsync(sqlConnection, jsonHeaders, _procName, jsonRoutes, body);
                        context.Result = _jsonAutoService.JsonPostContentResult(postResponse, _options.Mode, _options.ErrorThreshold);
                        _logger.LogInformation("Result retrieved");
                        break;
                    case nameof(SupportedMethods.DELETE):
                        _logger.LogInformation($"Executing DELETE request for {_procName} in db {sqlConnection}");
                        var deleteResponse = await _jsonAutoService.SqlDeleteAsync(sqlConnection, jsonHeaders, _procName, jsonRoutes);
                        context.Result = _jsonAutoService.JsonDeleteContentResult(deleteResponse, _options.Mode, _options.ErrorThreshold);
                        _logger.LogInformation("Result retrieved");
                        break;
                    case nameof(SupportedMethods.HEAD):
                        _logger.LogInformation($"Executing HEAD request for {_procName} in db {sqlConnection}");
                        var headResponse = await _jsonAutoService.SqlHeadAsync(sqlConnection, jsonHeaders, _procName, jsonRoutes);
                        context.Result = _jsonAutoService.JsonHeadContentResult((bool)headResponse);
                        _logger.LogInformation("Result retrieved");
                        break;
                    default:
                        context.Result = _jsonAutoService.JsonDefaultContentResult();
                        break;
                }
            }
        }
    }
}
