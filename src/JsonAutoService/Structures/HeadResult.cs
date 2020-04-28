using System;
using System.Data.SqlTypes;

namespace JsonAutoService.Structures
{
    public struct HeadResult : IRestResult
    {
        public bool IsValid
        {
            get => Bit;
            set => throw new NotImplementedException();
        }
        public bool Bit { get; }

        public HeadResult(SqlBoolean sqlBit)
        {
            Bit = (bool)sqlBit;
        }
    }
}
