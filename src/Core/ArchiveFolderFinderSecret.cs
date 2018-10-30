using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Core {
    public class ArchiveFolderFinderSecret : ISecret<CsScript> {
        private static CsScript vDefaultCsScript;
        public CsScript DefaultValue => vDefaultCsScript ?? (vDefaultCsScript = CreateDefaultCsScript());

        private static CsScript CreateDefaultCsScript() {
            var script = new CsScript(new List<CsScriptArgument> { new CsScriptArgument { Name = "folder", Value = "Full folder name" } },
                "public class ArchiveFolderFinder {",
                "public string FindArchiveFolder(string folder) {",
                "if (folder.EndsWith(\"\\\\\")) { folder = folder.Substring(0, folder.Length - 1); }",
                "var archiveFolder = folder + \"Archive\";",
                "if (!System.IO.Directory.Exists(archiveFolder)) { System.IO.Directory.CreateDirectory(archiveFolder); }",
                "return archiveFolder;",
                "}",
                "}",
                "var archiveFolderFinder = new ArchiveFolderFinder();",
                "var archiveFolder = archiveFolderFinder.FindArchiveFolder(folder);",
                "archiveFolder"
            ) {
                TimeoutInSeconds = 20
            };
            return script;
        }

        public string Guid => "A51B56B0-77D8-4D09-821F-60D449CF4F6C";
    }
}
