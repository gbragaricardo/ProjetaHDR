using ProjetaHDR.UI.ViewModels;
using System.Windows;

namespace ProjetaHDR.UI.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        internal LoginWindow(LoginViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.CloseWindow = () => this.Close();
        }
    }
}