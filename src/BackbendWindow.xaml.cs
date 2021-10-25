using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend {
    /// <summary>
    /// Interaction logic for BackbendWindow.xaml
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public partial class BackbendWindow {
        private IBackbendFoldersAnalyser BackbendFoldersAnalyser { get; }

        public BackbendWindow() {
            InitializeComponent();
            var builder = new ContainerBuilder().UseBackbend();
            var container = builder.Build();
            BackbendFoldersAnalyser = container.Resolve<IBackbendFoldersAnalyser>();
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e) {
            Environment.Exit(0);
        }

        private async void RefreshButton_OnClick(object sender, RoutedEventArgs e) {
            await RefreshAsync();
        }

        private async Task RefreshAsync() {
            var errorsAndInfos = new ErrorsAndInfos();
            var results = (await BackbendFoldersAnalyser.AnalyseAsync(errorsAndInfos)).Select(f => f.Folder.Name).ToList();
            results.InsertRange(0, errorsAndInfos.Errors);

            AnalysisResults.Text = string.Join("\r\n", results);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e) {
            await RefreshAsync();
        }

        private void Window_Closed(object sender, EventArgs e) {
            Environment.Exit(0);
        }
    }
}