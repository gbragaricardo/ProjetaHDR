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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjetaHDR.Commands.Waterproofing
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class WaterproofingCommand : RevitCommandBase, IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);

            var handler = new WaterproofingHandler();
            ExternalEvent externalEvent = ExternalEvent.Create(handler);

            var waterproofingTypeService = new WaterproofingTypeService(Context.Doc);
            var viewModel = new MainViewModel(waterproofingTypeService, externalEvent, handler);

            var window = new MainView (viewModel);

            window.Show();

            return Result.Succeeded;
        }
    }
}

