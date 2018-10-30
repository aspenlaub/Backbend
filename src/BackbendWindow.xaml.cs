using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;

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
            Environment.Exit(0);
        }

        private async void RefreshButton_OnClick(object sender, RoutedEventArgs e) {
            await RefreshAsync();
        }

        private async Task RefreshAsync() {
            var errorsAndInfos = new ErrorsAndInfos();
            var results = await BackbendFoldersAnalyser.AnalyseAsync(errorsAndInfos);
            var resultsList = results.ToList();
            resultsList.InsertRange(0, errorsAndInfos.Errors);

            AnalysisResults.Text = string.Join("\r\n", resultsList);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e) {
            await RefreshAsync();
        }

        private void Window_Closed(object sender, EventArgs e) {
            Environment.Exit(0);
        }
    }
}
