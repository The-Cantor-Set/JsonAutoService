using System.Collections.Generic;

namespace JsonAutoService.Service
{
    public class JsonAutoServiceOptions 
    {
        public string ConnectionString { get; set; }
        public string Mode { get; set;  }
        public int ErrorThreshold { get; set; }
        public IDictionary<string, string> DefaultErrorMessages { get; set; }
        public IDictionary<string, string> RequiredHeaders { get; set; }
        public IDictionary<string, string> IdentityClaims { get; set; }
    }
}
