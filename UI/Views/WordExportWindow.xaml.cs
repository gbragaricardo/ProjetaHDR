using ProjetaHDR.UI.ViewModels;
using System.Windows;

namespace ProjetaHDR.UI.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class WordExportWindow : Window
    {
        internal WordExportWindow(WordExportViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}