using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjetaHDR.OnStartup;
using ProjetaHDR.RevitContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.Commands
{
    internal class DiameterTagger : RevitCommandBase, IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);

            using (Transaction transacao = new Transaction(Context.Doc, "Tag Diametro"))
            {

                transacao.Start();

                

                transacao.Commit();

                return Result.Succeeded;
            }
        }
    }
}
