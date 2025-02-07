using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using ProjetaHDR.RevitContext;
using System.Runtime.Remoting.Contexts;
using ProjetaHDR.OnStartup;


namespace ProjetaHDR.Commands
{
    [Transaction(TransactionMode.Manual)]
    internal class Dev : RevitCommandBase, IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);
            UIBuilder.DevPushButton.Enabled = true;

            using (Transaction transacao = new Transaction(Context.Doc, "DEV"))
            {

                transacao.Start();

                TaskDialog.Show("DEV", "Teste");

                transacao.Commit();

                return Result.Succeeded;
            }
        }
    }
}