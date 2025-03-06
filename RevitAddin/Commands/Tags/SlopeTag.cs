using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjetaHDR.Startup;
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

        public double LengthFilterOption { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);
            LengthFilterOption = 0.2;

            using (Transaction transacao = new Transaction(Context.Doc, "Tag Inclinacao"))
            {
                transacao.Start();

                var unfilteredPipes = PipeUtils.GetPipesOnView(Context.Doc);
                if (unfilteredPipes.Count == 0 || unfilteredPipes == null)
                    return Result.Cancelled;


                var IsHydraulic = PipeUtils.HasPvcMarromPipes(Context.Doc, unfilteredPipes);

                if (IsHydraulic == true)
                {
                   return Result.Cancelled;
                }
                else
                {
                    var sanpipe = new FilterAndTagPipelines(Context.Doc, Context.Doc.ActiveView, "Inclinacao", LengthFilterOption, false);

                    sanpipe.PipelineSanitary(unfilteredPipes);
                    sanpipe.PipelineCreate();
                }

                transacao.Commit();
                return Result.Succeeded;
            }
        }
    }
}

