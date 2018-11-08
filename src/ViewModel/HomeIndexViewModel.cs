using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.ViewModel {
    public class HomeIndexViewModel {
        public string Title => "Backbend";

        public IEnumerable<ReasonAndBackbendFoldersToBeArchived> BackbendFoldersToBeArchivedPerReason;
    }

    public class ReasonAndBackbendFoldersToBeArchived {
        public string Reason { get; set; }
        public IEnumerable<BackbendFolder> Folders { get; set; } = new List<BackbendFolder>();
    }
}
