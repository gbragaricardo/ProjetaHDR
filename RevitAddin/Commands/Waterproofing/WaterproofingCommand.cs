using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.Commands.Waterproofing
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class WaterproofingCommand : RevitCommandBase, IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);

            MainView mainView = new MainView();
            mainView.ShowDialog();

            using (Transaction transacao = new Transaction(Context.Doc, "Tempo de Retorno em Areas"))
            {
            }
                return Result.Succeeded;
        }
    }
}
