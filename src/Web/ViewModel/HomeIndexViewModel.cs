using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Web.ViewModel {
    public class HomeIndexViewModel {
        public string Title => "Backbend";

        public IEnumerable<ReasonAndBackbendFoldersToBeArchived> BackbendFoldersToBeArchivedPerReason;
        public IEnumerable<string> Errors { get; set; } = new List<string>();
    }

    public class ReasonAndBackbendFoldersToBeArchived {
        public string Reason { get; set; }
        public IEnumerable<BackbendFolder> Folders { get; set; } = new List<BackbendFolder>();
    }
}
