using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjetaHDR.RevitAddin.RevitContext;
using ProjetaHDR.UI.ViewModels;
using ProjetaHDR.UI.Views;

namespace ProjetaHDR.Commands
{
    [Transaction(TransactionMode.Manual)]
    internal class ShowFamiliesPage : RevitCommandBase, IExternalCommand
    {
        public static RevitContext FamiliesContext { get; private set; }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);
            FamiliesContext = Context;
            RevitStaticContext.Update(Context.UiApp);

            DockablePaneId paneId = new DockablePaneId(new Guid("5072DFD8-7026-4CE6-A7C8-E56336EB21B2"));
            DockablePane pane = commandData.Application.GetDockablePane(paneId);

            if (pane != null)
            {
                pane.Show(); // Exibir o painel
            }

            return Result.Succeeded;
        }
    }
}
