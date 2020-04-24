using System;
using System.Data.SqlTypes;

namespace JsonAutoService.Structures
{
    public struct DeleteResult : IRestResult
    {
        public bool IsValid
        {
            get => Bit.IsNull ? false : (bool)Bit;
            set => throw new NotImplementedException();
        }

        public SqlBoolean Bit { get; }
        public SqlString Body { get; }

        public DeleteResult(SqlBoolean sqlBoolean, SqlString body)
        {
            Bit = sqlBoolean;
            Body = body;
        }
    }
}
