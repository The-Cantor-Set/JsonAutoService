using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace JsonAutoServiceExamples.Models
{
    public class OptionsModel
    {
        [JsonProperty("options")]
        public List<Product> Options { get; set; }
    }

    public class Option
    {
        [JsonProperty("o_id")]
        public long OptionId { get; set; }

        [JsonProperty("option_name")]
        public string OptionName { get; set; }

        [JsonProperty("created_dt")]
        public DateTime CreatedDate { get; set; }
    }
}
