using System.Linq;
using JsonAutoService.ResourceHandlers;
using JsonAutoService.Structures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using JsonAutoServiceExamples.Models;

namespace JsonAutoServiceExamples.Controllers
{
    [Route("api/jc")]
    [ApiController]
    public class JsonContextController : ControllerBase
    {
        // ------------------------------------------------------------------------- //
        // ------------------------------------------------------------------------- //
        // JsonAutoService context handler CRUD Controllers
        // JsonResourceContextHandler returns json prior to construction of the controller method
        // ------------------------------------------------------------------------- //
        // ------------------------------------------------------------------------- //

        [HttpGet("products")]
        [TypeFilter(typeof(JsonResourceContextHandler), Arguments = new object[] { "jas.api_products_get" })]
        public IActionResult GetProductsAsync()
        {
            var jAutoGetResult = (GetResult)HttpContext.Items[typeof(GetResult).Name];

            if (jAutoGetResult.IsValid)
            {
                var productsModel = JsonConvert.DeserializeObject<ProductsModel>(jAutoGetResult.Body.ToString());
                if (productsModel.Products.Any())
                {
                    return Ok(productsModel.Products.ToString());
                }
                return NotFound("No products");
            }
            return BadRequest("No bueno");
        }
    }
}