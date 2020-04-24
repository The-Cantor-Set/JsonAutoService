using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace JsonAutoServiceExamples.Models
{
    public class ProductsModel
    {
        [JsonProperty("products")]
        public List<Product> Products { get; set; }
    }

    public class Product
    {
        [JsonProperty("p_id")]
        public long ProductId { get; set; }

        [JsonProperty("product_name")]
        public string ProductName { get; set; }

        [JsonProperty("created_dt")]
        public DateTime CreatedDate { get; set; }
    }
}
