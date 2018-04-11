using System;
using System.Linq;
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

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e) {
            Refresh();
        }

        private void Refresh() {
            var errorsAndInfos = new ErrorsAndInfos();
            var results = BackbendFoldersAnalyser.Analyse(errorsAndInfos).ToList();
            results.InsertRange(0, errorsAndInfos.Errors);

            AnalysisResults.Text = string.Join("\r\n", results);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            Refresh();
        }

        private void Window_Closed(object sender, EventArgs e) {
            Environment.Exit(0);
        }
    }
}
