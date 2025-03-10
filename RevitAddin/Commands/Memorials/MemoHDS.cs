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
using ProjetaHDR.Utils;

namespace ProjetaHDR.Commands
{
    [Transaction(TransactionMode.Manual)]
    internal class MemoHDS : RevitCommandBase, IExternalCommand
    {
        public double LengthFilterOption { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);

            WordExportViewModel mmdViewModel = new WordExportViewModel(Context);
            WordExportWindow mmdWindow = new WordExportWindow(mmdViewModel);

            if (mmdWindow == null || mmdViewModel.ExportPath == null)
                return Result.Cancelled;


            mmdWindow.ShowDialog();


            return Result.Succeeded;

        }
    }
}
