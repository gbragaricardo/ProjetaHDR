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
            List<WaterproofingType> types = new List<WaterproofingType>();

            var floorTypes = new FilteredElementCollector(_doc)
                .OfClass(typeof(FloorType))
                .Cast<FloorType>()
                .Where(f => f.Name.ToLower().Contains("imp_"))
                .ToList();

            foreach (var ft in floorTypes)
            {
                double floorTypeThickness = ft.get_Parameter(BuiltInParameter.FLOOR_ATTR_DEFAULT_THICKNESS_PARAM)?.AsDouble() ?? 0;
                string floorTypeDescription = ft.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION)?.AsString() ?? ft.Name;
                double waterproofThicknessInMillimeters = UnitUtils.ConvertFromInternalUnits(floorTypeThickness, UnitTypeId.Millimeters);
                types.Add(new WaterproofingType
                {
                    Name = floorTypeDescription,
                    ElementTypeId = ft.Id,
                    Thickness = Math.Round(waterproofThicknessInMillimeters, 1)
                });
            }

            return types;
        }

        public ElementId CreateNewType(ElementId baseTypeId, string newTypeName)
        {
            return null;
        }
    }
}
