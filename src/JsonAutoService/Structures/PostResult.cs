using System;
using System.Data.SqlTypes;

namespace JsonAutoService.Structures
{
    public struct PostResult : IRestResult
    {
        public bool IsValid
        {
            get =>  Id.IsNull ? false : (bool)Id.ToSqlBoolean();
            set => throw new NotImplementedException();
        }

        public SqlInt64 Id { get; }
        public SqlString Body { get; }

        public PostResult(SqlInt64 id, SqlString body)
        {
            Id = id;
            Body = body;
        }
    }
}
