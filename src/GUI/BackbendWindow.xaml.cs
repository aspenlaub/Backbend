using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Components;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Repositories;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Helpers;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.GUI {
    /// <summary>
    /// Interaction logic for BackbendWindow.xaml
    /// </summary>
    public partial class BackbendWindow {
        private bool vNavigated;

        private Process vProcess;

        private readonly IDvinRepository vDvinRepository;

        public BackbendWindow() {
            InitializeComponent();
            vDvinRepository = new DvinRepository();
        }

        private void HtmlOutput_OnNavigated(object sender, NavigationEventArgs e) {
            Cursor = Cursors.Arrow;
            vNavigated = true;
        }

        private async void BackbendWindow_OnLoaded(object sender, RoutedEventArgs e) {
            NavigateToMessage($"Starting {Constants.BackbendAppId}&hellip;");

            var dvinApp = await vDvinRepository.LoadAsync(Constants.BackbendAppId);
            if (dvinApp == null) {
                NavigateToMessage($"{Constants.BackbendAppId} app not found");
                return;
            }

            Wait.Until(() => dvinApp.IsPortListenedTo(), TimeSpan.FromSeconds(5));
            if (!dvinApp.IsPortListenedTo() && !StartAppAndReturnSuccess(dvinApp)) {
                return;
            }

            Cursor = Cursors.Wait;
            Width = 720;
            Height = 720;
            HtmlOutput.Navigate("http://localhost:" + dvinApp.Port);
        }

        private bool StartAppAndReturnSuccess(IDvinApp dvinApp) {
            var fileSystemService = new FileSystemService();
            var errorsAndInfos = new ErrorsAndInfos();
            if (!dvinApp.HasAppBeenPublishedAfterLatestSourceChanges(Environment.MachineName, fileSystemService)) {
                dvinApp.Publish(fileSystemService, true, errorsAndInfos);
                if (errorsAndInfos.AnyErrors()) {
                    NavigateToMessage(string.Join("<br>", errorsAndInfos.Errors));
                    return false;
                }
            }

            if (!dvinApp.HasAppBeenPublishedAfterLatestSourceChanges(Environment.MachineName, fileSystemService)) {
                NavigateToMessage($"{Constants.BackbendAppId} has not been published since the latest source code changes");
                return false;
            }

            vProcess = dvinApp.Start(fileSystemService, errorsAndInfos);
            Wait.Until(() => dvinApp.IsPortListenedTo(), TimeSpan.FromSeconds(5));
            if (errorsAndInfos.AnyErrors()) {
                NavigateToMessage(string.Join("<br>", errorsAndInfos.Errors));
                return false;
            }

            if (dvinApp.IsPortListenedTo()) {
                return true;
            }

            NavigateToMessage($"{Constants.BackbendAppId} started but not listening");
            return false;
        }

        private void BackbendWindow_OnClosing(object sender, CancelEventArgs e) {
            try {
                vProcess?.Kill();
                // ReSharper disable once EmptyGeneralCatchClause
            } catch {
            }
        }

        private void NavigateToMessage(string message) {
            var markup = "<html><head></head><body><p>" + message + "</p></body></html>";
            HtmlOutput.NavigateToString(markup);
            Wait.Until(() => vNavigated, TimeSpan.FromSeconds(5));
        }
    }
}
