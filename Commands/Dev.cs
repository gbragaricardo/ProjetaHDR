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
using ProjetaHDR.Utils;
using Autodesk.Revit.DB.Plumbing;
using System.Xml.Linq;


namespace ProjetaHDR.Commands
{
    [Transaction(TransactionMode.Manual)]
    internal class Dev : RevitCommandBase, IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            View vistaAtiva = doc.ActiveView;

            using (Transaction trans = new Transaction(doc, "Testar Posição Relativa das Tags"))
            {
                trans.Start();

                trans.Commit();
            }

            return Result.Succeeded;
        }
    }
}