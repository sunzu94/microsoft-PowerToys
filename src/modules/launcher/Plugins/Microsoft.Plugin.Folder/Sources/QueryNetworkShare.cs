// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Plugin.Folder.Helpers;
using Microsoft.Plugin.Folder.Sources.Result;

namespace Microsoft.Plugin.Folder.Sources
{
    public class QueryNetworkShare : IQueryNetworkShare
    {
        public IEnumerable<IItemResult> Query(string search)
        {
            if (search == null)
            {
                throw new ArgumentNullException(nameof(search));
            }

            search = search.TrimEnd('\\');

            var shares = EnumNetShares(search);

            if (!shares.Any())
            {
                yield break;
            }

            yield return new CreateOpenCurrentFolderResult(search);

            var shareResult = BuildShareResult(search, shares);

            foreach (var result in shareResult)
            {
                yield return result;
            }
        }

        private static List<SHARE_INFO_1> EnumNetShares(string host)
        {
            List<SHARE_INFO_1> shareInfos = new List<SHARE_INFO_1>();
            int entriesRead = 0;
            int totalEntries = 0;
            int resumeHandle = 0;
            int nStructSize = Marshal.SizeOf(typeof(SHARE_INFO_1));
            IntPtr bufPtr = IntPtr.Zero;
            StringBuilder hostBuilder = new StringBuilder(host);
            uint size = 0xFFFFFFFF; // MAX_PREFERRED_LENGTH
            int result = NativeMethods.NetShareEnum(hostBuilder, 1, ref bufPtr, size, ref entriesRead, ref totalEntries, ref resumeHandle);

            // NERR_Success
            if (result == 0)
            {
                IntPtr currentPtr = bufPtr;
                for (int i = 0; i < entriesRead; i++)
                {
                    SHARE_INFO_1 shareInfo = (SHARE_INFO_1)Marshal.PtrToStructure(currentPtr, typeof(SHARE_INFO_1));

                    // STYPE_DISKTREE
                    if (shareInfo.shi1_type == 0)
                    {
                        shareInfos.Add(shareInfo);
                    }

                    currentPtr += nStructSize;
                }

                _ = NativeMethods.NetApiBufferFree(bufPtr);
            }

            return shareInfos;
        }

        private static IEnumerable<FolderItemResult> BuildShareResult(string search, IEnumerable<SHARE_INFO_1> shares)
        {
            return shares.Select(s => new FolderItemResult(search, s)).OrderBy(s => s.Title);
        }
    }
}
