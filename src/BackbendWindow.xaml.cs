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
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Helpers;
using Autofac;
using IContainer = Autofac.IContainer;
using TimeSpan = System.TimeSpan;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend {
    /// <summary>
    /// Interaction logic for BackbendWindow.xaml
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public partial class BackbendWindow {
        private bool vNavigated;

        private Process vProcess;

        private readonly IContainer vContainer;

        public BackbendWindow() {
            InitializeComponent();
            var builder = new ContainerBuilder().UseDvinAndPegh(new DummyCsArgumentPrompter());
            vContainer = builder.Build();
        }

        private void HtmlOutput_OnNavigated(object sender, NavigationEventArgs e) {
            Cursor = Cursors.Arrow;
            vNavigated = true;
        }

        private async void BackbendWindow_OnLoaded(object sender, RoutedEventArgs e) {
            await NavigateToMessage($"Starting {Constants.BackbendAppId}&hellip;");

            var errorsAndInfos = new ErrorsAndInfos();
            var dvinApp = await vContainer.Resolve<IDvinRepository>().LoadAsync(Constants.BackbendAppId, errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                await NavigateToMessage(errorsAndInfos.ErrorsToString());
                return;
            }
            if (dvinApp == null) {
                await NavigateToMessage($"{Constants.BackbendAppId} app not found");
                return;
            }

            Wait.Until(() => dvinApp.IsPortListenedTo(), TimeSpan.FromSeconds(5));
            if (!dvinApp.IsPortListenedTo() && !await StartAppAndReturnSuccess(dvinApp)) {
                return;
            }

            Cursor = Cursors.Wait;
            Width = 660;
            Height = 660;
            vNavigated = false;
            HtmlOutput.Navigate("http://localhost:" + dvinApp.Port);
        }

        private async Task<bool> StartAppAndReturnSuccess(IDvinApp dvinApp) {
            var fileSystemService = new FileSystemService();
            var errorsAndInfos = new ErrorsAndInfos();
            if (!dvinApp.HasAppBeenPublishedAfterLatestSourceChanges(fileSystemService)) {
                dvinApp.Publish(fileSystemService, true, errorsAndInfos);
                if (errorsAndInfos.AnyErrors()) {
                    await NavigateToMessage(string.Join("<br>", errorsAndInfos.Errors));
                    return false;
                }
            }

            if (!dvinApp.HasAppBeenPublishedAfterLatestSourceChanges(fileSystemService)) {
                await NavigateToMessage($"{Constants.BackbendAppId} has not been published since the latest source code changes");
                return false;
            }

            vProcess = dvinApp.Start(fileSystemService, errorsAndInfos);
            Wait.Until(() => dvinApp.IsPortListenedTo(), TimeSpan.FromSeconds(30));
            if (errorsAndInfos.AnyErrors()) {
                await NavigateToMessage(string.Join("<br>", errorsAndInfos.Errors));
                return false;
            }

            if (dvinApp.IsPortListenedTo()) {
                return true;
            }

            await NavigateToMessage($"{Constants.BackbendAppId} started but not listening");
            return false;
        }

        private void BackbendWindow_OnClosing(object sender, CancelEventArgs e) {
            try {
                vProcess?.Kill();
                // ReSharper disable once EmptyGeneralCatchClause
            } catch {
            }
        }

        private async Task NavigateToMessage(string message) {
            vNavigated = false;
            var markup = "<html><head></head><body><p>" + message + "</p></body></html>";
            HtmlOutput.NavigateToString(markup);
            await Task.Run(() => Wait.Until(() => vNavigated, TimeSpan.FromSeconds(5)));
        }
    }
}
