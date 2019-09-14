using System;
using System.Data.SqlTypes;

namespace JsonAutoService.Structures
{
    public struct GetResult : IRestResult
    {
        public bool IsValid
        {
            get => Convert.ToBoolean(Body.Length);
            set => throw new NotImplementedException();
        }

        public string Body { get; }

        public GetResult(SqlString body)
        {
            Body = (string)body;
        }
    }
}
