using MahApps.Metro.Controls;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjetaHDR.RevitAddin.Commands.Waterproofing.Views
{
    public partial class MainView : MetroWindow
    {
        public MainView(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.RequestHide += () => this.Hide();
            viewModel.RequestShow += () => this.Show();
            viewModel.RequestClose += () => this.Close();

        }
    }
}
