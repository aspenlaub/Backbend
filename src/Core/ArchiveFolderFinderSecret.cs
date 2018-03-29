using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Core {
    public class ArchiveFolderFinderSecret : ISecret<PowershellFunction<string, string>> {
        private static PowershellFunction<string, string> vDefaultPowershellFunction;
        public PowershellFunction<string, string> DefaultValue {
            get {
                return vDefaultPowershellFunction ??
                       (vDefaultPowershellFunction = new PowershellFunction<string, string> {
                           FunctionName = "Find-ArchiveFolder",
                           Script = "\r\n\tfunction Find-ArchiveFolder($folder) {\r\n"
                                    + "\t\tif ($folder.EndsWith(\"\\\")) {\r\n"
                                    + "\t\t\t$folder = $folder.Substring(0, $folder.Length - 1)\r\n"
                                    + "\t\t}\r\n"
                                    + "\t\t$archiveFolder = $folder + \"Archive\"\r\n"
                                    + "\t\t[System.IO.Directory]::CreateDirectory($archiveFolder) | Out-Null\r\n"
                                    + "\t\t$result = New-Object -TypeName \"Aspenlaub.Net.GitHub.CSharp.Pegh.Entities.PowershellFunctionResult\"\r\n"
                                    + "\t\t$result.Result = $archiveFolder\r\n"
                                    + "\t\treturn $result\r\n"
                                    + "\t}\r\n"
                       });
            }
        }

        public string Guid { get { return "822EE70E-1464-4342-906F-7DEC7E7AFDA6"; } }
    }
}
