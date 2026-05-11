using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ControlzEx.Standard;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.Services;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.ViewModels;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjetaHDR.Commands.Waterproofing
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class WaterproofingCommand : RevitCommandBase, IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);

            if (Context.Doc.ActiveView.GenLevel == null)
            {
                TaskDialog.Show("Atenção", "Por favor, execute este comando em uma vista de Planta.");
                return Result.Failed;
            }

            MainViewModel viewModel = new MainViewModel();
            MainView window = new MainView { DataContext = viewModel };

            bool? windowResult = window.ShowDialog();

            if (windowResult == true && viewModel.IsConfirmed)
            {

                try
                {
                    PickRegionsService pickRegionsService = new PickRegionsService();
                    IList<Reference> pickedRegionReferences = pickRegionsService.Pick(Context.UiDoc);

                    using (Transaction transaction = new Transaction(Context.Doc, "Converter Regiões em Pisos"))
                    {
                        transaction.Start();

                        ElementId levelId = Context.Doc.ActiveView.GenLevel.Id;

                        foreach (Reference regionRef in pickedRegionReferences)
                        {
                            Element regionElement = Context.Doc.GetElement(regionRef);

                            FilledRegion filledRegion = regionElement as FilledRegion;

                            if (filledRegion == null)
                                continue;

                            IList<CurveLoop> regionCurves = filledRegion.GetBoundaries();

                            Floor.Create(Context.Doc, regionCurves, /*viewModel.WaterproofingType.id*/ new ElementId((long)2147595), levelId);

                            Context.Doc.Delete(regionElement.Id);
                        }

                        transaction.Commit();
                    }

                    return Result.Succeeded;
                }
                catch (OperationCanceledException)
                {
                    return Result.Cancelled;
                }
            }

            return Result.Cancelled;
        }
    }
}

