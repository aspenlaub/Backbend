using System.IO;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Core {
    public class BackbendFoldersSecret : ISecret<BackbendFolders> {
        public static readonly string DefaultFolder = Path.GetTempPath() + @"AspenlaubTemp\Backbend\Test\";

        private BackbendFolders vBackbendFolders;
        public BackbendFolders DefaultValue => vBackbendFolders ?? (vBackbendFolders = new BackbendFolders { new BackbendFolder { Name = DefaultFolder } });

        public string Guid => "1F0CC5A2-C88A-4AEE-8A52-419F30FC1EE1";
    }
}
