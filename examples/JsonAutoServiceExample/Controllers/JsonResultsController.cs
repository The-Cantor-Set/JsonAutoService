using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonAutoService.ResourceHandlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JsonAutoServiceExamples.Controllers
{
    [Route("api/jr")]
    [ApiController]
    public class JsonResultsController : ControllerBase
    {
        // ------------------------------------------------------------------------- //
        // ------------------------------------------------------------------------- //
        // JsonAutoService results handler CRUD Controllers
        // ------------------------------------------------------------------------- //
        // ------------------------------------------------------------------------- //

        
        // ------------------------------------------------------------------------- //
        // Create (C__RUD) method(s)
        // ------------------------------------------------------------------------- //
        [HttpPost("product")]
        [TypeFilter(typeof(JsonResourceResultsHandler), Arguments = new object[] { "jas.api_product_post" })]
        public void PostProductAsync() { }

        [HttpPost("options")]
        [TypeFilter(typeof(JsonResourceResultsHandler), Arguments = new object[] { "jas.api_options_post" })]
        public void PostOptionsAsync() { }

        [HttpPost("pos")]
        [TypeFilter(typeof(JsonResourceResultsHandler), Arguments = new object[] { "jas.api_product_options_post" })]
        public void PostProductOptionsAsync() { }

        

        // ------------------------------------------------------------------------- //
        // Read (C__R__UD) method(s)
        // ------------------------------------------------------------------------- //
        [HttpGet("products")]
        [TypeFilter(typeof(JsonResourceResultsHandler), Arguments = new object[] { "jas.api_products_get" })]
        public void GetProductsAsync() { }

        [HttpGet("options")]
        [TypeFilter(typeof(JsonResourceResultsHandler), Arguments = new object[] { "jas.api_options_get" })]
        public void GetOptionsAsync() { }

        [HttpGet("po/{id}")]
        [TypeFilter(typeof(JsonResourceResultsHandler), Arguments = new object[] { "jas.api_product_option_get" })]
        public void GetProductOptionAsync(long id) { }

        [HttpGet("pos/p/{id}")]
        [TypeFilter(typeof(JsonResourceResultsHandler), Arguments = new object[] { "jas.api_pos_by_product_get" })]
        public void GetPosByProductAsync(long id) { }

        [HttpGet("pos/o/{id}")]
        [TypeFilter(typeof(JsonResourceResultsHandler), Arguments = new object[] { "jas.api_pos_by_option_get" })]
        public void GetPosByOptionAsync(long id) { }


        // ------------------------------------------------------------------------- //
        // Update (CR__U__D) method(s)
        // ------------------------------------------------------------------------- //
        [HttpPut("po/{id}")]
        [TypeFilter(typeof(JsonResourceResultsHandler), Arguments = new object[] { "jas.api_product_option_put" })]
        public void PutProductOptionAsync(long id) { }


        // ------------------------------------------------------------------------- //
        // Delete (CRU__D) method(s)
        // ------------------------------------------------------------------------- //
        [HttpDelete("po/{id}")]
        [TypeFilter(typeof(JsonResourceResultsHandler), Arguments = new object[] { "jas.api_product_option_delete" })]
        public void DeleteProductOptionAsync() { }
    }
}