using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.RevitAddin.Commands.Waterproofing.Services
{
    internal class RegionSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is FilledRegion region)
            {
                ElementId typeId = region.GetTypeId();

                ElementType regionType = elem.Document.GetElement(typeId) as ElementType;

                if (regionType != null && regionType.Name.ToLower().Contains("_imp"))
                    return true;
            }

            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
}
