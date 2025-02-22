using ProjetaHDR.UI.ViewModels;
using System.Windows;

namespace ProjetaHDR.UI.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class ExportMMD : Window
    {
        internal ExportMMD(ExportMMDViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
