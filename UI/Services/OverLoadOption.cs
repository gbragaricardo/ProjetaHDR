using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.UI.Services
{
    internal class OverLoadOption : IFamilyLoadOptions
    {
        public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
        {
            if (!familyInUse)
            {
                overwriteParameterValues = true;
                return true;
            }
            TaskDialog.Show("Aviso", "A família está em uso e será substituída");
            overwriteParameterValues = true;
            return true;
        }

        public bool OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
        {
            if (!familyInUse)
            {
                source = FamilySource.Family;
                overwriteParameterValues = true;
                return true;
            }
            TaskDialog.Show("Aviso", "A família está em uso e será substituída");
            source = FamilySource.Family;
            overwriteParameterValues = true;
            return true;
        }
    }
}
