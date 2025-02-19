using System.Windows;
using System.Windows.Controls;
using Autodesk.Revit.UI;
using ProjetaHDR.UI.ViewModels;

namespace ProjetaHDR.UI.Views
{
    /// <summary>
    /// Interaction logic for FamiliesView.xaml
    /// </summary>
    public partial class FamiliesView : UserControl, IDockablePaneProvider
    {
        internal FamiliesView()
        {
            InitializeComponent();
            DataContext = new FamiliesViewModel();
        }

        public void SetupDockablePane(DockablePaneProviderData data)
        {
            data.VisibleByDefault = false;
            data.FrameworkElement = (FrameworkElement) this; // Define a UI que será exibida no painel
            data.InitialState = new DockablePaneState
            {
                DockPosition = DockPosition.Right // Define a posição do painel
            };
        }
    }
}
