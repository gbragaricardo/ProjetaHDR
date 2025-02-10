using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ProjetaHDR.Utils
{ 
    public static class PipeFittingUtils
    {
        /// <summary>
        /// Seleciona todas as conexões de tubo no projeto.
        /// </summary>
        public static IList<Element> SelecionarConexoes(Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_PipeFitting)
                .WhereElementIsNotElementType()
                .ToElements();
        }
    }
}
