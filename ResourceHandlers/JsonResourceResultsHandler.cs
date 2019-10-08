using JsonAutoService.Service;
using JsonAutoService.Structures;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
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
        private readonly ILogger _logger;
        private readonly string _procName;

        public JsonResourceResultsHandler(IOptionsMonitor<JsonAutoServiceOptions> options, IJsonAutoService jsonAutoService, object procName, ILogger logger)
        {
            _options = options.CurrentValue;
            _jsonAutoService = jsonAutoService;
            _logger = logger;
            _procName = procName.ToString();
            
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            _logger.LogInformation("Entered OnResourceExecutionAsync() method of JsonResourceResultsHandler");
            Console.WriteLine("Entered OnResourceExecutionAsync() method of JsonResourceResultsHandler");
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
            
            // debug
            var stopWatch = new Stopwatch();

            //Handle request
            using (var reader = new StreamReader(request.Body))
            {
                var body = await reader.ReadToEndAsync();

                switch (request.Method)
                {
                    case nameof(SupportedMethods.GET):
                        _logger.LogInformation($"Executing GET request for {_procName} in db {sqlConnection}");
                        Console.WriteLine($"Executing GET request for {_procName} in db {sqlConnection}");
                        stopWatch.Start();
                        var getResponse = await _jsonAutoService.SqlGetAsync(sqlConnection, jsonHeaders, _procName, jsonRoutes);
                        context.Result = _jsonAutoService.JsonGetContentResult((string)getResponse);
                        stopWatch.Stop();
                        var ts1 = stopWatch.Elapsed;
                        var elapsedTime1 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                            ts1.Hours, ts1.Minutes, ts1.Seconds,
                            ts1.Milliseconds / 10);
                        _logger.LogInformation($"Result retrieved in {elapsedTime1}");
                        Console.WriteLine($"Result retrieved in {elapsedTime1}");
                        break;
                    case nameof(SupportedMethods.PUT):
                        _logger.LogInformation($"Executing PUT request for {_procName} in db {sqlConnection}");
                        Console.WriteLine($"Executing PUT request for {_procName} in db {sqlConnection}");
                        stopWatch.Start();
                        var putResponse = await _jsonAutoService.SqlPutAsync(sqlConnection, jsonHeaders, _procName, jsonRoutes, body);
                        context.Result = _jsonAutoService.JsonPutContentResult(putResponse, _options.Mode, _options.ErrorThreshold);
                        stopWatch.Stop();
                        var ts2 = stopWatch.Elapsed;
                        var elapsedTime2 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                            ts2.Hours, ts2.Minutes, ts2.Seconds,
                            ts2.Milliseconds / 10);
                        _logger.LogInformation($"Result retrieved in {elapsedTime2}");
                        Console.WriteLine($"Result retrieved in {elapsedTime2}");
                        break;
                    case nameof(SupportedMethods.POST):
                        _logger.LogInformation($"Executing POST request for {_procName} in db {sqlConnection}");
                        Console.WriteLine();
                        var postResponse = await _jsonAutoService.SqlPostAsync(sqlConnection, jsonHeaders, _procName, jsonRoutes, body);
                        context.Result = _jsonAutoService.JsonPostContentResult(postResponse, _options.Mode, _options.ErrorThreshold);
                        stopWatch.Stop();
                        var ts3 = stopWatch.Elapsed;
                        var elapsedTime3 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                            ts3.Hours, ts3.Minutes, ts3.Seconds,
                            ts3.Milliseconds / 10);
                        _logger.LogInformation($"Result retrieved in {elapsedTime3}");
                        Console.WriteLine($"Result retrieved in {elapsedTime3}");
                        break;
                    case nameof(SupportedMethods.DELETE):
                        _logger.LogInformation($"Executing DELETE request for {_procName} in db {sqlConnection}");
                        Console.WriteLine();
                        var deleteResponse = await _jsonAutoService.SqlDeleteAsync(sqlConnection, jsonHeaders, _procName, jsonRoutes);
                        context.Result = _jsonAutoService.JsonDeleteContentResult(deleteResponse, _options.Mode, _options.ErrorThreshold);
                        stopWatch.Stop();
                        var ts4 = stopWatch.Elapsed;
                        var elapsedTime4 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                            ts4.Hours, ts4.Minutes, ts4.Seconds,
                            ts4.Milliseconds / 10);
                        _logger.LogInformation($"Result retrieved in {elapsedTime4}");
                        Console.WriteLine($"Result retrieved in {elapsedTime4}");
                        break;
                    case nameof(SupportedMethods.HEAD):
                        _logger.LogInformation($"Executing HEAD request for {_procName} in db {sqlConnection}");
                        Console.WriteLine();
                        var headResponse = await _jsonAutoService.SqlHeadAsync(sqlConnection, jsonHeaders, _procName, jsonRoutes);
                        context.Result = _jsonAutoService.JsonHeadContentResult((bool)headResponse);
                        stopWatch.Stop();
                        var ts5 = stopWatch.Elapsed;
                        var elapsedTime5 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                            ts5.Hours, ts5.Minutes, ts5.Seconds,
                            ts5.Milliseconds / 10);
                        _logger.LogInformation($"Result retrieved in {elapsedTime5}");
                        Console.WriteLine($"Result retrieved in {elapsedTime5}");
                        break;
                    default:
                        context.Result = _jsonAutoService.JsonDefaultContentResult();
                        break;
                }
            }
        }
    }
}
