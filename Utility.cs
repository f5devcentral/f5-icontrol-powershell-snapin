using System;
using System.Collections.Generic;
using System.Text;

namespace iControlSnapIn
{
    public class Utility
    {
        public static UInt64 build64(iControl.CommonULong64 ul64)
        {
            UInt64 value = ((UInt64)(UInt32)ul64.high) << 32 | ((UInt64)(UInt32)ul64.low);
            return value;
        }

    }
}
