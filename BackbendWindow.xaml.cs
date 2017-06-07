using System.Windows;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend {
    /// <summary>
    /// Interaction logic for BackbendWindow.xaml
    /// </summary>
    public partial class BackbendWindow {
        public BackbendWindow() {
            InitializeComponent();
        }

        private void Close_OnClick(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
