using System;
using System.Data.SqlTypes;

namespace JsonAutoService.Structures
{
    public struct PutResult : IRestResult
    {
        public bool IsValid
        {
            get => Bit;
            set => throw new NotImplementedException();
        }

        public bool Bit { get; }
        public string Body { get; }

        public PutResult(SqlBoolean sqlBit, SqlString body)
        {
            Bit = (bool)sqlBit;
            Body = (string)body;
        }
    }
}
