using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ProjetaHDR.Commands
{
    [Transaction(TransactionMode.Manual)]
    internal class SetAreasTR: RevitCommandBase, IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);

            using (Transaction transacao = new Transaction(Context.Doc, "Nested Families"))
            {
                transacao.Start();

                SetAreaReturnTime();

                transacao.Commit();

                return Result.Succeeded;
            }
        }

        private void SetAreaReturnTime()
        {
            IList<AreaScheme> areaSchemes = new FilteredElementCollector(Context.Doc)
                .OfCategory(BuiltInCategory.OST_AreaSchemes)
                .Where(a =>    a.Id.Value == 23802931 // Cobertura
                            || a.Id.Value == 23802949) // Terreo

                .Select(a => a as AreaScheme)
                .ToList();

            var areasList = new FilteredElementCollector(Context.Doc)
                .OfCategory(BuiltInCategory.OST_Areas)
                .WhereElementIsNotElementType()
                .ToElements();

            foreach (var elementArea in areasList)
            {
                if (elementArea is Area area)
                {
                    var returnTimeParam = area.get_Parameter(new Guid("fd39346e-f1b6-42bc-8996-ab8a606aa983"));
                    if (area.AreaScheme.Id.Value == 23802931) // Cobertura
                        returnTimeParam.Set(25);

                    else if (area.AreaScheme.Id.Value == 23802949) // Cobertura
                        returnTimeParam.Set(5);

                }
            }
        }
    }
}
