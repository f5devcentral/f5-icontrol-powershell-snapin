using System;
using System.Collections.Generic;
using System.Text;

namespace iControlSnapIn
{
    public class Utilities
    {
        public static string Secure2Clear(System.Security.SecureString secString)
        {
            string clearString = "";

            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToCoTaskMemUnicode(secString);
            clearString = System.Runtime.InteropServices.Marshal.PtrToStringUni(ptr);
            System.Runtime.InteropServices.Marshal.ZeroFreeCoTaskMemUnicode(ptr);

            return clearString;
        }
    }
}
