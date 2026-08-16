using CommunityToolkit.Mvvm.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using webNetWorkDriver.model;
using webNetWorkDriver.View;
using webNetWorkDriver.ViewModel;


namespace webNetWorkDriver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CssService _cssService = new CssService();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(_cssService);
        }
        // Add a button click handler to open StyleViewerWindow
        private void OpenStyleViewer_Click(object sender, RoutedEventArgs e)
        {
            var styleViewerVm = new StyleViewerViewModel(_cssService);
            var styleViewerWindow = new StyleViewer(styleViewerVm);
            styleViewerWindow.Show();
        }

    }
}