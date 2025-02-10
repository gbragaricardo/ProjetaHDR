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
    internal class FlowTag : RevitCommandBase, IExternalCommand
    {
        public double LengthFilterOption { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);


            using (Transaction transacao = new Transaction(Context.Doc, "Tag Fluxo"))
            {
                transacao.Start();

                var unfilteredPipes = PipeFilters.GetPipesOnView(Context.Doc);
                var IsHydraulic = PipeFilters.HasPvcMarromPipes(Context.Doc, unfilteredPipes);

                if (IsHydraulic == true)
                {
                    LengthFilterOption = 0.1;

                    unfilteredPipes = PipeFilters.GetPvcMarromPipes(Context.Doc, unfilteredPipes);

                    var hydPipe = new FilterAndTagPipelines(Context.Doc, Context.Doc.ActiveView, "Flow", LengthFilterOption, true);

                    hydPipe.PipelineHydraulic(unfilteredPipes);
                    hydPipe.PipelineFlow();
                }
                else
                {
                    LengthFilterOption = 0.2;

                    var sanpipe = new FilterAndTagPipelines(Context.Doc, Context.Doc.ActiveView, "Flow", LengthFilterOption, false);

                    sanpipe.PipelineSanitary(unfilteredPipes);
                    sanpipe.PipelineFlow();
                }

                transacao.Commit();
                return Result.Succeeded;
            }
        }
    }
}
