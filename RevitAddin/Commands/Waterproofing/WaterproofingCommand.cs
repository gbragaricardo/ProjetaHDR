using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.Services;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.ViewModels;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.Views;
using System;
using System.Windows.Interop;

namespace ProjetaHDR.Commands.Waterproofing
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class WaterproofingCommand : RevitCommandBase, IExternalCommand
    {
        private static MainView _window;
        private static MainViewModel _viewModel;
        private static string _docPathName;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);

            if (_docPathName == null)
                _docPathName = Context.Doc.PathName;

            if (_viewModel != null && Context.Doc.PathName != _docPathName)
            {
                _viewModel = null;
                _window = null;
                _docPathName = Context.Doc.PathName;
            }

            if (_viewModel == null)
            {
                WaterproofingTypeService waterproofingTypeService = new WaterproofingTypeService(Context.Doc);
                _viewModel = new MainViewModel(waterproofingTypeService);
            }

            if (_window == null || !_window.IsVisible)
            {
                _window = new MainView(_viewModel);

                IntPtr revitHandle = Context.UiApp.MainWindowHandle;
                new WindowInteropHelper(_window).Owner = revitHandle;

                _window.Closed += (s, e) => _window = null;
                _window.Show();
            }
            else
            {
                if (_window.WindowState == System.Windows.WindowState.Minimized)
                    _window.WindowState = System.Windows.WindowState.Normal;

                _window.Activate();
                _window.Focus();
            }

            return Result.Succeeded;
        }
    }
}

