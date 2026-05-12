using Autodesk.Revit.DB;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.RevitAddin.Commands.Waterproofing.Services
{
    public class WaterproofingTypeService
    {
        private readonly Document _doc;
        public WaterproofingTypeService(Document doc)
        {
            _doc = doc;
        }

        public List<WaterproofingType> GetAvailableTypes()
        {
            List<WaterproofingType> tipos = new List<WaterproofingType>();

            var floorTypes = new FilteredElementCollector(_doc)
                .OfClass(typeof(FloorType))
                .Cast<FloorType>()
                .ToList();

            foreach (var ft in floorTypes)
            {
                tipos.Add(new WaterproofingType
                {
                    Name = ft.Name,
                    ElementTypeId = ft.Id // Guarda o ID dinâmico real deste projeto!
                });
            }

            return tipos; // Retorna essa lista para a sua ViewModel preencher o ComboBox
        }

        public ElementId CreateNewType(ElementId baseTypeId, string newTypeName)
        {
            return null;
        }
    }
}
