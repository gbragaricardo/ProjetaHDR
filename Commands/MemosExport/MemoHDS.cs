using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjetaHDR.Commands.Helpers;
using ProjetaHDR.Commands.MemosExport.Helpers;
using ProjetaHDR.RevitContext;
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


            using (Transaction transacao = new Transaction(Context.Doc, "MMD HDS"))
            {
                transacao.Start();

                var exportPath = DocHandler.ObterCaminhoSalvar();



                DocHandler.CarregarDocumento(exportPath);

                var modifier = new WordReplacer(Context.Doc, Context.Doc.ProjectInformation, exportPath);

                modifier.ProjectInfoReplaces();
                



                DocHandler.AbrirDocumento(exportPath);

                transacao.Commit();
                return Result.Succeeded;
            }
        }
    }
}
