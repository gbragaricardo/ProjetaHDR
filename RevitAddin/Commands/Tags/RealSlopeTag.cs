using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjetaHDR.Utils;

namespace ProjetaHDR.Commands
{
    [Transaction(TransactionMode.Manual)]
    internal class RealSlopeTag : RevitCommandBase, IExternalCommand
    {

        public double LengthFilterOption { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);
            LengthFilterOption = 0.2;

            using (Transaction transacao = new Transaction(Context.Doc, "Tag Inclinacao Real"))
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
                    var sanpipe = new FilterAndTagPipelines(Context.Doc, Context.Doc.ActiveView, "Inclinacao Real", LengthFilterOption, false);

                    sanpipe.PipelineSanitary(unfilteredPipes);
                    sanpipe.PipelineCreate();
                }

                transacao.Commit();
                return Result.Succeeded;
            }
        }
    }
}

