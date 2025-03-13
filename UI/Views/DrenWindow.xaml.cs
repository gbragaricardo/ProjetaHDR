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
using Autodesk.Revit.DB;
using ProjetaHDR.UI.ViewModels;

namespace ProjetaHDR.UI.Views
{
    /// <summary>
    /// Interaction logic for DrenWindow.xaml
    /// </summary>
    public partial class DrenWindow : Window
    {
        internal DrenWindow(DrenViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
