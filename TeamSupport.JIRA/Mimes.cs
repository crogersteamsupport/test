using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.JIRA
{
    public static class Mimes
    {

        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        public extern static uint FindMimeFromData(uint pBC,
        [MarshalAs(UnmanagedType.LPStr)] string pwzUrl,
        [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
        uint cbSize,
        [MarshalAs(UnmanagedType.LPStr)] string pwzMimeProposed,
        uint dwMimeFlags,
        out uint ppwzMimeOut,
        uint dwReserverd);
    }
}
