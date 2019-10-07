﻿using JsonAutoService.Service;
using JsonAutoService.Structures;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Serilog;

namespace JsonAutoService.ResourceHandlers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class JsonResourceResultsHandler : Attribute, IAsyncResourceFilter
    {
        private readonly JsonAutoServiceOptions _options;
        private readonly IJsonAutoService _jsonAutoService;
        private readonly string _procName;

        public JsonResourceResultsHandler(IOptionsMonitor<JsonAutoServiceOptions> options, IJsonAutoService jsonAutoService, object procName)
        {
            _options = options.CurrentValue;
            _jsonAutoService = jsonAutoService;
            _procName = procName.ToString();
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            Log.Information("Entered OnResourceExecutionAsync() method of JsonResourceResultsHandler");
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
                        Log.Information($"Executing GET request for {_procName} in db {sqlConnection}");
                        var getResponse = await _jsonAutoService.SqlGetAsync(sqlConnection, jsonHeaders, _procName, jsonRoutes);
                        context.Result = _jsonAutoService.JsonGetContentResult((string)getResponse);
                        Log.Information("Result retrieved");
                        break;
                    case nameof(SupportedMethods.PUT):
                        Log.Information($"Executing PUT request for {_procName} in db {sqlConnection}");
                        var putResponse = await _jsonAutoService.SqlPutAsync(sqlConnection, jsonHeaders, _procName, jsonRoutes, body);
                        context.Result = _jsonAutoService.JsonPutContentResult(putResponse, _options.Mode, _options.ErrorThreshold);
                        Log.Information("Result retrieved");
                        break;
                    case nameof(SupportedMethods.POST):
                        Log.Information($"Executing POST request for {_procName} in db {sqlConnection}");
                        var postResponse = await _jsonAutoService.SqlPostAsync(sqlConnection, jsonHeaders, _procName, jsonRoutes, body);
                        context.Result = _jsonAutoService.JsonPostContentResult(postResponse, _options.Mode, _options.ErrorThreshold);
                        Log.Information("Result retrieved");
                        break;
                    case nameof(SupportedMethods.DELETE):
                        Log.Information($"Executing DELETE request for {_procName} in db {sqlConnection}");
                        var deleteResponse = await _jsonAutoService.SqlDeleteAsync(sqlConnection, jsonHeaders, _procName, jsonRoutes);
                        context.Result = _jsonAutoService.JsonDeleteContentResult(deleteResponse, _options.Mode, _options.ErrorThreshold);
                        Log.Information("Result retrieved");
                        break;
                    case nameof(SupportedMethods.HEAD):
                        Log.Information($"Executing HEAD request for {_procName} in db {sqlConnection}");
                        var headResponse = await _jsonAutoService.SqlHeadAsync(sqlConnection, jsonHeaders, _procName, jsonRoutes);
                        context.Result = _jsonAutoService.JsonHeadContentResult((bool)headResponse);
                        Log.Information("Result retrieved");
                        break;
                    default:
                        context.Result = _jsonAutoService.JsonDefaultContentResult();
                        break;
                }
            }
        }
    }
}
