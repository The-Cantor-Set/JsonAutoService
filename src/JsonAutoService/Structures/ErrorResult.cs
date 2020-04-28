using Newtonsoft.Json;

namespace JsonAutoService.Structures
{
    public struct ErrorResult
    {
        [JsonProperty("error_number")]
        public int ErrorNumber { get; set; }

        [JsonProperty("error_severity")]
        public int ErrorSeverity { get; set; }

        [JsonProperty("error_line")]
        public int ErrorLine { get; set; }

        [JsonProperty("error_procedure")]
        public string ErrorProcedure { get; set; }

        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }

        [JsonProperty("xact_state")]
        public int XactState { get; set; }
    }
}
