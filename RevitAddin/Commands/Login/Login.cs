using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjetaHDR.RevitContext;
using ProjetaHDR.UI;
using ProjetaHDR.UI.ViewModels;
using ProjetaHDR.UI.Views;
using ProjetaHDR.Utils;

namespace ProjetaHDR.Commands
{
    [Transaction(TransactionMode.Manual)]
    internal class Login : RevitCommandBase, IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);

            
            LoginViewModel viewModel = new LoginViewModel(Context);
            LoginWindow loginWindow = new LoginWindow(viewModel);

            loginWindow.ShowDialog();

            return Result.Succeeded;
        }
    }
}
