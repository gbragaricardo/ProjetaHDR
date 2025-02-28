using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjetaHDR.UI.ViewModels;
using ProjetaHDR.UI.Views;

namespace ProjetaHDR.Commands
{
    [Transaction(TransactionMode.Manual)]
    internal class Login : RevitCommandBase, IExternalCommand
    {
        internal static LoginViewModel ViewModel { get; private set; }
        internal static LoginWindow Window { get; private set; }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);

            if (ViewModel == null)
                ViewModel = new LoginViewModel(Context);

            if (Window == null || !Window.IsVisible)
            {
                Window = new LoginWindow(ViewModel);
                Window.ShowDialog();
            }
            else
            {
                Window.Focus();
            }

            return Result.Succeeded;
        }
    }
}