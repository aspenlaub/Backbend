using System.Windows;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend {
    /// <summary>
    /// Interaction logic for BackbendWindow.xaml
    /// </summary>
    public partial class BackbendWindow {
        private BackbendFoldersAnalyser BackbendFoldersAnalyser { get; }
        private ComponentProvider ComponentProvider { get; }

        public BackbendWindow() {
            InitializeComponent();
            ComponentProvider = new ComponentProvider();
            BackbendFoldersAnalyser = new BackbendFoldersAnalyser(ComponentProvider);
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e) {
            Close();
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e) {
            Refresh();
        }

        private void Refresh() {
            var results = BackbendFoldersAnalyser.Analyse();
            AnalysisResults.Text = string.Join("\r\n", results);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            Refresh();
        }
    }
}
