﻿using JsonAutoService.Service;
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
        private ILogger<JsonResourceResultsHandler> _logger;
        private readonly string _procName;

        public JsonResourceResultsHandler(IOptionsMonitor<JsonAutoServiceOptions> options, IJsonAutoService jsonAutoService, object procName, ILogger<JsonResourceResultsHandler> logger)
        {
            _options = options.CurrentValue;
            _jsonAutoService = jsonAutoService;
            _logger = logger;
            _procName = procName.ToString();
            
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            _logger.LogDebug("Entered OnResourceExecutionAsync() method of JsonResourceResultsHandler");
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
                        _logger.LogDebug($"Executing GET request for {_procName} in db {sqlConnection}");
                        var getResponse = await _jsonAutoService.SqlGetAsync(sqlConnection, jsonHeaders, _procName, jsonRoutes);
                        context.Result = _jsonAutoService.JsonGetContentResult((string)getResponse);
                        _logger.LogDebug("Result retrieved");
                        break;
                    case nameof(SupportedMethods.PUT):
                        _logger.LogDebug($"Executing PUT request for {_procName} in db {sqlConnection}");
                        var putResponse = await _jsonAutoService.SqlPutAsync(sqlConnection, jsonHeaders, _procName, jsonRoutes, body);
                        context.Result = _jsonAutoService.JsonPutContentResult(putResponse, _options.Mode, _options.ErrorThreshold);
                        _logger.LogDebug("Result retrieved");
                        break;
                    case nameof(SupportedMethods.POST):
                        _logger.LogDebug($"Executing POST request for {_procName} in db {sqlConnection}");
                        var postResponse = await _jsonAutoService.SqlPostAsync(sqlConnection, jsonHeaders, _procName, jsonRoutes, body);
                        context.Result = _jsonAutoService.JsonPostContentResult(postResponse, _options.Mode, _options.ErrorThreshold);
                        _logger.LogDebug("Result retrieved");
                        break;
                    case nameof(SupportedMethods.DELETE):
                        _logger.LogDebug($"Executing DELETE request for {_procName} in db {sqlConnection}");
                        var deleteResponse = await _jsonAutoService.SqlDeleteAsync(sqlConnection, jsonHeaders, _procName, jsonRoutes);
                        context.Result = _jsonAutoService.JsonDeleteContentResult(deleteResponse, _options.Mode, _options.ErrorThreshold);
                        _logger.LogDebug("Result retrieved");
                        break;
                    case nameof(SupportedMethods.HEAD):
                        _logger.LogDebug($"Executing HEAD request for {_procName} in db {sqlConnection}");
                        var headResponse = await _jsonAutoService.SqlHeadAsync(sqlConnection, jsonHeaders, _procName, jsonRoutes);
                        context.Result = _jsonAutoService.JsonHeadContentResult((bool)headResponse);
                        _logger.LogDebug("Result retrieved");
                        break;
                    default:
                        context.Result = _jsonAutoService.JsonDefaultContentResult();
                        break;
                }
            }
        }
    }
}
