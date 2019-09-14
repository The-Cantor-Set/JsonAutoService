using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Security.Claims;
using System.Threading.Tasks;
using JsonAutoService.Structures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JsonAutoService.Service
{
    public interface IJsonAutoService
    {
        Task<SqlString> SqlGetAsync(string conString, string jsonHeaders, string procName, string jsonParams);
        Task<PutResult> SqlPutAsync(string conString, string jsonHeaders, string procName, string jsonParams, string jsonBody);
        Task<PostResult> SqlPostAsync(string conString, string jsonHeaders, string procName, string jsonParams, string jsonBody);
        Task<DeleteResult> SqlDeleteAsync(string conString, string jsonHeaders, string procName, string jsonParams);
        Task<SqlBoolean> SqlHeadAsync(string conString, string jsonHeaders, string procName, string jsonParams);

        ContentResult JsonPostContentResult(PostResult postResult, string mode, int errorThreshold);
        ContentResult JsonPutContentResult(PutResult putResult, string mode, int errorThreshold);
        ContentResult JsonDeleteContentResult(DeleteResult deleteResult, string mode, int errorThreshold);
        ContentResult JsonGetContentResult(string getResult);
        ContentResult JsonHeadContentResult(bool headResult);
        ContentResult JsonDefaultContentResult();
        ContentResult JsonBadRequestContentResult(string errorMessage);

        Dictionary<string, object> GetHeadersDictionary(IHeaderDictionary headers, ClaimsPrincipal user);

        Task<ContentResult> ActionContextResultAsync(ActionContext context, string procName, IDictionary<string, object> addToHeaderse = null, string body = null);
    }
}
