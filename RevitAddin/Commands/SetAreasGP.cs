using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ProjetaHDR.Commands
{
    [Transaction(TransactionMode.Manual)]
    internal class SetAreasGP : RevitCommandBase, IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);

            using (Transaction transacao = new Transaction(Context.Doc, "Parametros de intensidade em Areas"))
            {
                transacao.Start();

                SetAreasGlobalParameters();

                transacao.Commit();

                return Result.Succeeded;
            }
        }

        private void SetAreasGlobalParameters()
        {
            var areasList = new FilteredElementCollector(Context.Doc)
                .OfCategory(BuiltInCategory.OST_Areas)
                .WhereElementIsNotElementType()
                .ToElements();

            IList<GlobalParameter> globalParameters = new FilteredElementCollector(Context.Doc)
                .OfClass(typeof(GlobalParameter))
                .Cast<GlobalParameter>()
                .ToList();

            foreach (var area in areasList)
            {
                var coefParam = area.get_Parameter(new Guid("bee6c53d-9ca0-4a45-8fab-a710e1884587"));
                if (coefParam.HasValue == false)
                    coefParam.Set(1);

                Parameter a = area.get_Parameter(new Guid("c9688612-5497-43b6-b36e-bf7559036bec"));
                Parameter b = area.get_Parameter(new Guid("80506c0c-c8cd-43bb-9895-70a89ea83433"));
                Parameter c = area.get_Parameter(new Guid("5cf801e8-6c6d-411b-a076-69d66f1b93fd"));
                Parameter k = area.get_Parameter(new Guid("c8022a50-c2ba-4d9c-982e-97cbfc536fb6"));
                Parameter IntensidadePluvio = area.get_Parameter(new Guid("e6222830-0cd5-4005-b208-75d285474908"));

                var returnTimeParam = area.get_Parameter(new Guid("fd39346e-f1b6-42bc-8996-ab8a606aa983"));

                a.AssociateWithGlobalParameter(globalParameters.FirstOrDefault(gp => gp.Name == "A").Id);
                b.AssociateWithGlobalParameter(globalParameters.FirstOrDefault(gp => gp.Name == "B").Id);
                c.AssociateWithGlobalParameter(globalParameters.FirstOrDefault(gp => gp.Name == "C").Id);
                k.AssociateWithGlobalParameter(globalParameters.FirstOrDefault(gp => gp.Name == "K").Id);

                //switch (returnTimeParam.AsInteger())
                //{
                //    case 5: IntensidadePluvio.AssociateWithGlobalParameter(globalParameters.FirstOrDefault(gp => gp.Name == "Intensidade 05 Anos").Id);
                //            break;

                //    case 25: IntensidadePluvio.AssociateWithGlobalParameter(globalParameters.FirstOrDefault(gp => gp.Name == "Intensidade Cobertura").Id);
                //             break;

                //    default: break;
                //}
               
            }
        }
    }
}
