using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using System.Runtime.Remoting.Contexts;
using ProjetaHDR.Startup;
using ProjetaHDR.Utils;
using Autodesk.Revit.DB.Plumbing;
using System.Xml.Linq;
using ProjetaHDR.UI.ViewModels;
using ProjetaHDR.UI.Views;


namespace ProjetaHDR.Commands
{
    [Transaction(TransactionMode.Manual)]
    internal class Dev : RevitCommandBase, IExternalCommand
    {
        internal static DrenViewModel ViewModel { get; private set; }
        internal static DrenWindow Window { get; private set; }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);

            if (ViewModel != null)
            {
                if (!ViewModel.Context.Doc.IsValidObject)
                {
                    ViewModel = null;
                    Window = null;
                }
                else if (ViewModel.Context.Doc.CreationGUID != Context.Doc.CreationGUID)
                {
                    ViewModel = null;
                    Window = null;
                }
            }

            if (ViewModel == null)
                ViewModel = new DrenViewModel(Context);

            ViewModel.Context = Context;
            ViewModel.LoadFixtureList();
            ViewModel.ValidateFixtureItems();

            if (Window == null || !Window.IsVisible)
            {
                Window = new DrenWindow(ViewModel);
                Window.ShowDialog();
            }
            else
            {
                Window.Focus();
            }

            ViewModel.SaveDataStorage();

            return Result.Succeeded;
        }

    }
}