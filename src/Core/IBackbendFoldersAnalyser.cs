using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Core;

public interface IBackbendFoldersAnalyser {
    Task<IEnumerable<BackbendFolderToBeArchived>> AnalyseAsync(IErrorsAndInfos errorsAndInfos);
}