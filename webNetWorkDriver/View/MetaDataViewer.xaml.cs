using System.Windows;
using webNetWorkDriver.ViewModel;

namespace webNetWorkDriver.View
{
    /// <summary>
    /// Interaction logic for MetaDataViewer.xaml
    /// </summary>
    public partial class MetaDataViewer : Window
    {
        public MetaDataViewer(MetaDataViewerViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
