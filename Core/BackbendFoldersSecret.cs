using System;
using System.IO;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Core {
    public class BackbendFoldersSecret : ISecret<BackbendFolders> {
        public static readonly string DefaultFolder = Path.GetTempPath() + @"\Temp\Backbend\Test\";

        private BackbendFolders vBackbendFolders;
        public BackbendFolders DefaultValue {
            get {
                return vBackbendFolders ?? (vBackbendFolders = new BackbendFolders { new BackbendFolder { Name = DefaultFolder, Machine = Environment.MachineName } });
            }
        }

        public string Guid { get { return "1F0CC5A2-C88A-4AEE-8A52-419F30FC1EE1"; } }
    }
}
