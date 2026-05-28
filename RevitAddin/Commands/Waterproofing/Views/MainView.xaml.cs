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
        private MainViewModel _viewModel;
        public MainView(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            viewModel.RequestHide += this.Hide;
            viewModel.RequestShow += this.Show;
            viewModel.RequestClose += this.Close;
            this.Closed += MainView_Closed;
        }

        private void MainView_Closed(object sender, EventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.RequestHide -= this.Hide;
                _viewModel.RequestShow -= this.Show;
                _viewModel.RequestClose -= this.Close;
            }
        }

        private void ComboBox_OnDropDownOpened(object sender, EventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.UpdateWaterproofingFloorTypes();
            }
        }
    }
}
