using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using ProjetaHDR.UI.ViewModels;
using ProjetaHDR.UI.Views;
using System.Windows.Interop;


namespace ProjetaHDR.Commands
{
    [Transaction(TransactionMode.Manual)]
    internal class Dev : RevitCommandBase, IExternalCommand
    {
        internal static DrenViewModel ViewModel { get; private set; }
        internal static DrenWindow Window { get; private set; }
        public static RevitContext HelperContext { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);
            HelperContext = Context;

            IntPtr revitHandle = Context.UiApp.MainWindowHandle;

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

            ////////PARA TESTE
            //ViewModel = null;
            //Window = null;
            ////////PARA TESTE

            if (ViewModel == null)
                ViewModel = new DrenViewModel(Context);

            ViewModel.Context = Context;
            ViewModel.LoadFixtureList();
            ViewModel.ValidateFixtureItems();

            if (Window == null || !Window.IsVisible)
            {
                Window = new DrenWindow(ViewModel);

                WindowInteropHelper helper = new WindowInteropHelper(Window);
                helper.Owner = revitHandle;

                Window.Show();
            }
            else
            {
                Window.Close();
            }

            ViewModel.SaveDataStorage();

            return Result.Succeeded;
        }

    }
}