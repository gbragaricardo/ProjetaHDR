using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ControlzEx.Standard;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.Events;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.Services;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.ViewModels;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interop;

namespace ProjetaHDR.Commands.Waterproofing
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class WaterproofingCommand : RevitCommandBase, IExternalCommand
    {
        private static MainView _window;
        private static MainViewModel _viewModel;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);

            var handler = new WaterproofingHandler();
            ExternalEvent externalEvent = ExternalEvent.Create(handler);

            var waterproofingTypeService = new WaterproofingTypeService(Context.Doc);

            if (_window != null && _window.IsVisible)
            {
                _window.Activate();
                _window.Focus();
                return Result.Succeeded;
            }

            if (_viewModel == null)
                _viewModel = new MainViewModel(waterproofingTypeService, externalEvent, handler);

            _window = new MainView(_viewModel);
            _window.Show();

            IntPtr revitHandle = Context.UiApp.MainWindowHandle;
            new WindowInteropHelper(_window).Owner = revitHandle;

            _window.Closed += (s, e) => _window = null;

            return Result.Succeeded;
        }
    }
}

