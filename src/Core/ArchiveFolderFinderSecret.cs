using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Core {
    public class ArchiveFolderFinderSecret : ISecret<CsLambda> {
        private static CsLambda vDefaultCsLambda;
        public CsLambda DefaultValue => vDefaultCsLambda ?? (vDefaultCsLambda = CreateDefaultCsLambda());

        private static CsLambda CreateDefaultCsLambda() {
            var csLambda = new CsLambda {
                LambdaExpression = @"folder => {
                    if (folder.EndsWith(""\\"")) { folder = folder.Substring(0, folder.Length - 1); }
                    var archiveFolder = folder + ""Archive"";
                    if (!Directory.Exists(archiveFolder)) { Directory.CreateDirectory(archiveFolder); }
                    return archiveFolder;
                }"
            };
            csLambda.Namespaces.Add("System.IO");
            csLambda.Types.Add("System.IO.Directory");
            return csLambda;
        }

        public string Guid => "77D8A510-B56B-4D09-821F-60D449CF4F6C";
    }
}
