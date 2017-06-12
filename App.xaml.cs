namespace Aspenlaub.Net.GitHub.CSharp.Backbend {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public class App {
        public static string SourceFileFullName() {
            return SourceFileFullNameOfCaller();
        }

        private static string SourceFileFullNameOfCaller([System.Runtime.CompilerServices.CallerFilePath] string sourceFileFullName = "") {
            return sourceFileFullName;
        }
    }
}
