using System;
using System.Collections.Generic;
using System.Text;

namespace CSDBCReader
{
    public class DBCException : Exception
    {
        public DBCException(string message)
            : base(message)
        {   
        }
    }
}
