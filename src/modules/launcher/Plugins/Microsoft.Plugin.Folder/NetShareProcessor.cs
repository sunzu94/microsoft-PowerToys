// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Plugin.Folder.Sources;
using Microsoft.Plugin.Folder.Sources.Result;

namespace Microsoft.Plugin.Folder
{
    public class NetShareProcessor : IFolderProcessor
    {
        private readonly IFolderHelper _folderHelper;
        private readonly IQueryNetworkShare _networkShare;

        public NetShareProcessor(IFolderHelper folderHelper, IQueryNetworkShare networkShare)
        {
            _folderHelper = folderHelper;
            _networkShare = networkShare;
        }

        public IEnumerable<IItemResult> Results(string actionKeyword, string search)
        {
            if (!_folderHelper.IsNetworkShare(search))
            {
                return Enumerable.Empty<IItemResult>();
            }

            return _networkShare.Query(search);
        }
    }
}
