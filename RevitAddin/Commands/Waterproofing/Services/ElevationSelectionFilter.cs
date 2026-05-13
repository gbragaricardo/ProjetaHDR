using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.RevitAddin.Commands.Waterproofing.Services
{
    public class ElevationSelectionFilter : ISelectionFilter
    {
        private Document _doc;

        public ElevationSelectionFilter(Document doc)
        {
            _doc = doc;
        }

        public bool AllowElement(Element elem)
        {
            if (elem is FilledRegion) return false;

            if (elem is RevitLinkInstance) return true;

            return IsValidCategory(elem);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            if (reference.LinkedElementId != ElementId.InvalidElementId)
            {
                var linkInstance = _doc.GetElement(reference.ElementId) as RevitLinkInstance;
                var linkDoc = linkInstance.GetLinkDocument();
                var linkedElement = linkDoc.GetElement(reference.LinkedElementId);

                return IsValidCategory(linkedElement);
            }

            return true;
        }

        private bool IsValidCategory(Element elem)
        {
            if (elem.Category == null) return false;

            int categoryId = (int)elem.Category.Id.Value;

            return categoryId == (int)BuiltInCategory.OST_Floors ||
                   categoryId == (int)BuiltInCategory.OST_StructuralFraming;
        }
    }
}