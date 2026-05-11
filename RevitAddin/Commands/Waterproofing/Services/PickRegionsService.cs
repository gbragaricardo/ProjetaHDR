using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace ProjetaHDR.RevitAddin.Commands.Waterproofing.Services
{
    internal class PickRegionsService
    {
        public IList<Reference> Pick(UIDocument uiDoc)
        {
            Selection selectionContext = uiDoc.Selection;

            return selectionContext.PickObjects(ObjectType.Element, new RegionSelectionFilter(), "Selecione as regiões para conversão");
        }
    }
}
