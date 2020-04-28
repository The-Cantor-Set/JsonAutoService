using System;
using System.Data.SqlTypes;

namespace JsonAutoService.Structures
{
    public struct GetResult : IRestResult
    {
        public bool IsValid
        {
            get => !Body.IsNull;
            set => throw new NotImplementedException();
        }

        public SqlString Body { get; }

        public GetResult(SqlString body)
        {
            Body = body;
        }
    }
}
