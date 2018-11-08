namespace Aspenlaub.Net.GitHub.CSharp.Backbend.GUI {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App {
        public static string SourceFileFullName() {
            return SourceFileFullNameOfCaller();
        }

        private static string SourceFileFullNameOfCaller([System.Runtime.CompilerServices.CallerFilePath] string sourceFileFullName = "") {
            return sourceFileFullName;
        }
    }
}
