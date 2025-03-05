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
    internal class DiameterTag : RevitCommandBase, IExternalCommand
    {
        public double LengthFilterOption { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);
            

            using (Transaction transacao = new Transaction(Context.Doc, "Tag Diametro"))
            {
                transacao.Start();

                var unfilteredPipes = PipeUtils.GetPipesOnView(Context.Doc);
                var IsHydraulic = PipeUtils.HasPvcMarromPipes(Context.Doc, unfilteredPipes);

                if(IsHydraulic == true)
                {
                    if (Context.ActiveView is View3D)
                        LengthFilterOption = 0.15;
                    else
                        LengthFilterOption = 0.1;

                    unfilteredPipes = PipeUtils.GetPvcMarromPipes(Context.Doc, unfilteredPipes);

                    var hydPipe = new FilterAndTagPipelines(Context.Doc, Context.Doc.ActiveView, "Diametro", LengthFilterOption, true);

                    hydPipe.PipelineHydraulic(unfilteredPipes);
                    hydPipe.PipelineCreate();
                }
                else
                {
                    LengthFilterOption = 0.2;

                    var sanpipe = new FilterAndTagPipelines(Context.Doc, Context.Doc.ActiveView, "Diametro", LengthFilterOption, false);

                    sanpipe.PipelineSanitary(unfilteredPipes);
                    sanpipe.PipelineCreate();
                }

                transacao.Commit();
                return Result.Succeeded;
            }
        }
    }
}
