// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Plugin.Folder.Helpers
{
    public static class NativeMethods
    {
        [DllImport("netapi32.dll")]
        internal static extern int NetApiBufferFree(IntPtr Buffer);

        [DllImport("netapi32.dll", CharSet = CharSet.Unicode)]
        internal static extern int NetShareEnum(StringBuilder servername, int level, ref IntPtr bufptr, uint prefmaxlen, ref int entriesread, ref int totalentries, ref int resume_handle);
    }
}
