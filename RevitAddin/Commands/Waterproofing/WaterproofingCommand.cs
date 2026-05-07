using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.Services;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.Commands.Waterproofing
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class WaterproofingCommand : RevitCommandBase, IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);

            //MainView mainView = new MainView();
            //mainView.ShowDialog();

            PickRegions pickRegionsService = new PickRegions();

            IList<Reference> pickedRegions = pickRegionsService.Pick(Context.UiDoc);

            using (Transaction transaction = new Transaction(Context.Doc, "Conveter regioes em piso de imp"))
            {

                foreach (Reference refRegion in pickedRegions)
                {
                    Element regionElement = Context.Doc.GetElement(refRegion);

                    FilledRegion filledRegion = regionElement as FilledRegion;

                    if (filledRegion == null)
                        continue;

                    IList<CurveLoop> regionCurves = filledRegion.GetBoundaries();

                    transaction.Start();

                    Floor.Create(Context.Doc, regionCurves, new ElementId((long)2147595), ((View)Context.Doc.ActiveView).GenLevel.Id);
                    Context.Doc.Delete(regionElement.Id);

                    transaction.Commit();
                }
            }

            return Result.Succeeded;
        }
    }
}
