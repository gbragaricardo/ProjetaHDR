using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ProjetaHDR
{
    internal class FixtureFamilyItem
    {
        public string Name { get; set; }
        public ElementId Id { get; set; }
        public Element InstanceElement { get; set; }
    }

    
}
