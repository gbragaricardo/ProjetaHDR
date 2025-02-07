using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjetaHDR.OnStartup;
using ProjetaHDR.RevitContext;
using ProjetaHDR.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.Commands
{
    [Transaction(TransactionMode.Manual)]
    internal class SlopeTag : RevitCommandBase, IExternalCommand
    {
        public IList<Element> SanitaryPipes { get; set; }
        public double LengthFilterOption { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);

            LengthFilterOption = 0.2;

            using (Transaction transacao = new Transaction(Context.Doc, "Tag Inclinacao"))
            {
                SanitaryPipes = FilterPipelines.PipelineSanitary(Context.Doc, LengthFilterOption, "Inclinacao");

                transacao.Start();
                PipeUtils.SetPipeSlope(SanitaryPipes);
                TagManager.DeleteExistingTags(Context.Doc, SanitaryPipes, FilterPipelines.TagId);
                TagManager.CreateTags(Context.Doc, SanitaryPipes, FilterPipelines.TagId, FilterPipelines.SanitaryInsertPoints);

                transacao.Commit();
                return Result.Succeeded;
            }
        }
    }
}

