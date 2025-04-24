using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.UI.Services
{
    internal class SelectionOptions : ISelectionFilter
    {
        private readonly BuiltInCategory _category;

        public SelectionOptions(BuiltInCategory category) => _category = category;
        
        public bool AllowElement(Element element)
        {
            if (element.Category == null) return false;

            if (element.Category.GetHashCode() == ((int)_category)) return true;

            return false;
        }

        public bool AllowReference(Reference reference, XYZ position) => false;
    
    }

}
