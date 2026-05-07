using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.RevitAddin.Commands.Waterproofing.Models
{
    internal class WaterproofingType
    {
        public string Name { get; set; }
        public ElementId ElementTypeId{ get; set; }
    }
}
