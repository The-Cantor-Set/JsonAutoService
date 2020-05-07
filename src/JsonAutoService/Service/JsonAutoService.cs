using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using JsonAutoService.Structures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace JsonAutoService.Service
{
    /// <summary>
    /// 
    /// The Database First (DF) API framework for .NET Core
    /// 
    /// </summary>
    public partial class JsonAutoService : IJsonAutoService
    {
        private const string Json = "application/json";

        private readonly ILogger<JsonAutoServiceOptions> _logger;
        private readonly JsonAutoServiceOptions _options;

        public JsonAutoService(IOptionsMonitor<JsonAutoServiceOptions> options, ILogger<JsonAutoServiceOptions> logger)
        {
            _logger = logger;
            this._options = options.CurrentValue;
        }

        public async Task<SqlString> SqlGetAsync(string conString, string jsonHeaders, string procName, string jsonParams)
        {
            using (var sqlCon = new SqlConnection(conString))
            using (var sqlCmd = new SqlCommand(procName, sqlCon))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;

                var h = sqlCmd.Parameters.Add("@headers", SqlDbType.NVarChar, -1);
                h.Direction = ParameterDirection.Input;
                h.Value = jsonHeaders;
                var p = sqlCmd.Parameters.Add("@params", SqlDbType.NVarChar, -1);
                p.Direction = ParameterDirection.Input;
                p.Value = jsonParams;

                await sqlCon.OpenAsync();
                using (var sqlDataReader = await sqlCmd.ExecuteReaderAsync())
                {
                    if (sqlDataReader.HasRows)
                    {
                        SqlString sqlString = SqlString.Null;
                        while (await sqlDataReader.ReadAsync())
                        {
                            sqlString = (sqlString.IsNull) ? sqlDataReader.GetSqlString(0).Value : SqlString.Add(sqlString, sqlDataReader.GetSqlString(0).Value);
                        }
                        return sqlString;
                    }
                    return SqlString.Null;
                }
            }
        }

        public async Task<PutResult> SqlPutAsync(string conString, string jsonHeaders, string procName, string jsonParams, string jsonBody)
        {
            using (var sqlCon = new SqlConnection(conString))
            using (var sqlCmd = new SqlCommand(procName, sqlCon))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;

                var h = sqlCmd.Parameters.Add("@headers", SqlDbType.NVarChar, -1);
                h.Direction = ParameterDirection.Input;
                h.Value = jsonHeaders;
                var p = sqlCmd.Parameters.Add("@params", SqlDbType.NVarChar, -1);
                p.Direction = ParameterDirection.Input;
                p.Value = jsonParams;
                var b = sqlCmd.Parameters.Add("@body", SqlDbType.NVarChar, -1);
                b.Direction = ParameterDirection.Input;
                b.Value = jsonBody;

                var t = sqlCmd.Parameters.Add("@test_value", SqlDbType.Bit);
                t.Direction = ParameterDirection.Output;
                var r = sqlCmd.Parameters.Add("@response", SqlDbType.NVarChar, -1);
                r.Direction = ParameterDirection.Output;

                await sqlCon.OpenAsync();
                await sqlCmd.ExecuteNonQueryAsync();
                sqlCon.Close();
                return new PutResult((SqlBoolean)t.SqlValue, (SqlString)r.SqlValue);
            }
        }

        public async Task<PostResult> SqlPostAsync(string conString, string jsonHeaders, string procName, string jsonParams, string jsonBody)
        {
            _logger.LogInformation($"Sql Post params: {conString}, {jsonHeaders}, {procName}, {jsonParams}, {jsonBody}");

            using (var sqlCon = new SqlConnection(conString))
            using (var sqlCmd = new SqlCommand(procName, sqlCon))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;

                var h = sqlCmd.Parameters.Add("@headers", SqlDbType.NVarChar, -1);
                h.Direction = ParameterDirection.Input;
                h.Value = jsonHeaders;
                var p = sqlCmd.Parameters.Add("@params", SqlDbType.NVarChar, -1);
                p.Direction = ParameterDirection.Input;
                p.Value = jsonParams;
                var b = sqlCmd.Parameters.Add("@body", SqlDbType.NVarChar, -1);
                b.Direction = ParameterDirection.Input;
                b.Value = jsonBody;

                var t_id = sqlCmd.Parameters.Add("@test_id", SqlDbType.BigInt);
                t_id.Direction = ParameterDirection.Output;
                var r = sqlCmd.Parameters.Add("@response", SqlDbType.NVarChar, -1);
                r.Direction = ParameterDirection.Output;

                await sqlCon.OpenAsync();
                await sqlCmd.ExecuteNonQueryAsync();
                sqlCon.Close();
                return new PostResult((SqlInt64)t_id.SqlValue, (SqlString)r.SqlValue);
            }
        }

        public async Task<PostResult> SqlPostAsync(ActionContext context, string jsonHeaders, string procName, string jsonParams, string jsonBody)
        {
            _logger.LogInformation($"Sql Post params: {jsonHeaders}, {procName}, {jsonParams}, {jsonBody}");

            var conString = _options.ConnectionString;
            //var conString = context.HttpContext.Items[_options.ConnectionStringName].ToString();

            using (var sqlCon = new SqlConnection(conString))
            using (var sqlCmd = new SqlCommand(procName, sqlCon))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;

                var h = sqlCmd.Parameters.Add("@headers", SqlDbType.NVarChar, -1);
                h.Direction = ParameterDirection.Input;
                h.Value = jsonHeaders;
                var p = sqlCmd.Parameters.Add("@params", SqlDbType.NVarChar, -1);
                p.Direction = ParameterDirection.Input;
                p.Value = jsonParams;
                var b = sqlCmd.Parameters.Add("@body", SqlDbType.NVarChar, -1);
                b.Direction = ParameterDirection.Input;
                b.Value = jsonBody;

                var t_id = sqlCmd.Parameters.Add("@test_id", SqlDbType.BigInt);
                t_id.Direction = ParameterDirection.Output;
                var r = sqlCmd.Parameters.Add("@response", SqlDbType.NVarChar, -1);
                r.Direction = ParameterDirection.Output;

                await sqlCon.OpenAsync();
                await sqlCmd.ExecuteNonQueryAsync();
                sqlCon.Close();
                return new PostResult((SqlInt64)t_id.SqlValue, (SqlString)r.SqlValue);
            }
        }

        public async Task<DeleteResult> SqlDeleteAsync(string conString, string jsonHeaders, string procName, string jsonParams)
        {
            using (var sqlCon = new SqlConnection(conString))
            using (var sqlCmd = new SqlCommand(procName, sqlCon))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;

                var h = sqlCmd.Parameters.Add("@headers", SqlDbType.NVarChar, -1);
                h.Direction = ParameterDirection.Input;
                h.Value = jsonHeaders;
                var p = sqlCmd.Parameters.Add("@params", SqlDbType.NVarChar, -1);
                p.Direction = ParameterDirection.Input;
                p.Value = jsonParams;

                var t = sqlCmd.Parameters.Add("@test_value", SqlDbType.Bit);
                t.Direction = ParameterDirection.Output;
                var r = sqlCmd.Parameters.Add("@response", SqlDbType.NVarChar, -1);
                r.Direction = ParameterDirection.Output;

                await sqlCon.OpenAsync();
                await sqlCmd.ExecuteNonQueryAsync();
                sqlCon.Close();
                return new DeleteResult((SqlBoolean)t.SqlValue, (SqlString)r.SqlValue);
            }
        }

        public async Task<SqlBoolean> SqlHeadAsync(string conString, string jsonHeaders, string procName, string jsonParams)
        {
            using (var sqlCon = new SqlConnection(conString))
            using (var sqlCmd = new SqlCommand(procName, sqlCon))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;

                var h = sqlCmd.Parameters.Add("@headers", SqlDbType.NVarChar, -1);
                h.Direction = ParameterDirection.Input;
                h.Value = jsonHeaders;
                var p = sqlCmd.Parameters.Add("@params", SqlDbType.NVarChar, -1);
                p.Direction = ParameterDirection.Input;
                p.Value = jsonParams;

                await sqlCon.OpenAsync();
                var reply = await sqlCmd.ExecuteScalarAsync();
                sqlCon.Close();
                return (SqlBoolean)reply;
            }
        }

        public ContentResult JsonPostContentResult(PostResult postResponse, string mode, int errorThreshold)
        {
            if (postResponse.IsValid)
            {
                return new ContentResult
                {
                    StatusCode = StatusCodes.Status201Created,
                    Content = postResponse.Body.ToString(),
                    ContentType = Json
                };
            }
            else
            {
                var errorResult = JsonConvert.DeserializeObject<ErrorResult>(postResponse.Body.ToString());
                var errorMessage = ErrorMessage(mode, errorThreshold, errorResult, SupportedMethods.POST);

                return new ContentResult
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Content = JsonConvert.SerializeObject(errorMessage),
                    ContentType = Json
                };
            }
        }

        public ContentResult JsonPutContentResult(PutResult putResponse, string mode, int errorThreshold)
        {
            if (putResponse.IsValid)
            {
                return new ContentResult
                {
                    StatusCode = StatusCodes.Status201Created,
                    Content = putResponse.Body.ToString(),
                    ContentType = Json
                };
            }
            else
            {
                var errorResult = JsonConvert.DeserializeObject<ErrorResult>(putResponse.Body.ToString());
                var errorMessage = ErrorMessage(mode, errorThreshold, errorResult, SupportedMethods.PUT);

                return new ContentResult
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Content = JsonConvert.SerializeObject(errorMessage),
                    ContentType = Json
                };
            }
        }

        public ContentResult JsonDeleteContentResult(DeleteResult deleteResponse, string mode, int errorThreshold)
        {
            if (deleteResponse.IsValid)
            {
                return new ContentResult
                {
                    StatusCode = StatusCodes.Status201Created,
                    Content = deleteResponse.Body.ToString(),
                    ContentType = Json
                };
            }
            else
            {
                var errorResult = JsonConvert.DeserializeObject<ErrorResult>(deleteResponse.Body.ToString());
                var errorMessage = ErrorMessage(mode, errorThreshold, errorResult, SupportedMethods.DELETE);

                return new ContentResult
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Content = JsonConvert.SerializeObject(errorMessage),
                    ContentType = Json
                };
            }
        }

        public ContentResult JsonGetContentResult(string getResponse = null)
        {
            if (getResponse == "Null")
            {
                return new ContentResult
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }
            return new ContentResult
            {
                ContentType = Json,
                StatusCode = StatusCodes.Status200OK,
                Content = (!String.IsNullOrEmpty(getResponse)) ? getResponse : JsonConvert.SerializeObject(_options.DefaultErrorMessages[SupportedMethods.GET])
            };
        }

        public ContentResult JsonHeadContentResult(bool headResponse)
        {
            return new ContentResult
            {
                StatusCode = headResponse ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest
            };
        }

        public ContentResult JsonDefaultContentResult()
        {
            return new ContentResult
            {
                StatusCode = StatusCodes.Status405MethodNotAllowed,
                Content = JsonConvert.SerializeObject(new { errorMessage = _options.DefaultErrorMessages["Default"] }),
                ContentType = Json
            };
        }

        public ContentResult JsonBadRequestContentResult(string errorMessage)
        {
            return new ContentResult
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Content = JsonConvert.SerializeObject(errorMessage),
                ContentType = Json
            };
        }

        public async Task<ContentResult> ActionContextResultAsync(ActionContext context, string procName, IDictionary<string, object> addToHeaders=null, string body=null)
        {
            var httpContext = context.HttpContext;
            var request = httpContext.Request;
            var user = context.HttpContext.User;

            //Serialize Routes
            var jsonRoutes = JsonConvert.SerializeObject(context.RouteData.Values);

            //Serialize Headers
            var headerDictionary = GetHeadersDictionary(request.Headers, user);
            if (addToHeaders != null)
                addToHeaders.ToList().ForEach(h => headerDictionary.Add(h.Key, h.Value));
            var jsonHeaders = JsonConvert.SerializeObject(headerDictionary);

            var conString = _options.ConnectionString;
            //var conString = httpContext.Items[_options.ConnectionStringName].ToString();

            // execute sql based on method type and return contentResult
            switch (httpContext.Request.Method)
            {
                case nameof(SupportedMethods.GET):
                    var sqlGetResult = new GetResult(await SqlGetAsync(conString, jsonHeaders, procName, jsonRoutes));
                    return JsonGetContentResult(sqlGetResult.Body.ToString());
                case nameof(SupportedMethods.PUT):
                    var sqlPutResult = await SqlPutAsync(conString, jsonHeaders, procName, jsonRoutes, body);
                    return JsonPutContentResult(sqlPutResult, _options.Mode, _options.ErrorThreshold);
                case nameof(SupportedMethods.POST):
                    var sqlPostResult = await SqlPostAsync(conString, jsonHeaders, procName, jsonRoutes, body);
                    return JsonPostContentResult(sqlPostResult, _options.Mode, _options.ErrorThreshold);
                case nameof(SupportedMethods.DELETE):
                    var sqlDeleteResult = await SqlDeleteAsync(conString, jsonHeaders, procName, jsonRoutes);
                    return JsonDeleteContentResult(sqlDeleteResult, _options.Mode, _options.ErrorThreshold);
                case nameof(SupportedMethods.HEAD):
                    var sqlHeadResult = new HeadResult(await SqlHeadAsync(conString, jsonHeaders, procName, jsonRoutes));
                    return JsonHeadContentResult(sqlHeadResult.Bit);
                default:
                    return JsonDefaultContentResult();
            }
        }
        
        private string ErrorMessage(string mode, int errorThreshold, ErrorResult errorResult, string httpMethod)
        {
            var errorMessageDefault = _options.DefaultErrorMessages[httpMethod];

            switch (mode)
            {
                case "Default":
                    return errorMessageDefault;
                case "Passthrough":
                    return errorResult.ErrorNumber.Equals(_options.ErrorThreshold) ? errorResult.ErrorMessage : errorMessageDefault;
                case "Debug":
                    return errorResult.ErrorMessage;
                default:
                    return errorMessageDefault;
            }
        }

        public Dictionary<string, object> GetHeadersDictionary(IHeaderDictionary headers, ClaimsPrincipal user)
        {
            var headerPairs = new Dictionary<string, object>();
            if (user.Identity.IsAuthenticated)
            {
                //Add identity from ClaimsPrincipal
                foreach (var identityHeader in _options.IdentityClaims)
                {
                    headerPairs.Add(identityHeader.Value, user.FindFirst(identityHeader.Value).Value);
                }
            }
            
            foreach (var header in headers)
            {
                // transpose key/value if a required header
                if (_options.RequiredHeaders.ContainsKey(header.Key))
                {
                    headerPairs.Add(_options.RequiredHeaders[header.Key].ToString(), (string)header.Value);
                }

            }
            
            return headerPairs;
        }
    }
}
