// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using Microsoft.Plugin.Folder.Helpers;
using Wox.Infrastructure;
using Wox.Plugin;

namespace Microsoft.Plugin.Folder.Sources.Result
{
    public class FolderItemResult : IItemResult
    {
        private static readonly IShellAction ShellAction = new ShellAction();

        public FolderItemResult()
        {
        }

        public FolderItemResult(DisplayFileInfo fileSystemInfo)
        {
            Title = fileSystemInfo.Name;
            Subtitle = fileSystemInfo.FullName;
            Path = fileSystemInfo.FullName;
        }

        public FolderItemResult(string host, SHARE_INFO_1 shareInfo)
        {
            var path = System.IO.Path.Combine(host, shareInfo.shi1_netname);

            Title = shareInfo.shi1_netname;
            Subtitle = path;
            Path = path;
        }

        public string Title { get; set; }

        public string Subtitle { get; set; }

        public string Path { get; set; }

        public string Search { get; set; }

        public Wox.Plugin.Result Create(IPublicAPI contextApi)
        {
            return new Wox.Plugin.Result(StringMatcher.FuzzySearch(Search, Title).MatchData)
            {
                Title = Title,
                IcoPath = Path,

                // Using CurrentCulture since this is user facing
                SubTitle = string.Format(CultureInfo.CurrentCulture, Properties.Resources.wox_plugin_folder_select_folder_result_subtitle, Subtitle),
                QueryTextDisplay = Path,
                ContextData = new SearchResult { Type = ResultType.Folder, FullPath = Path },
                Action = c => ShellAction.Execute(Path, contextApi),
            };
        }
    }
}
