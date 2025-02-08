using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.Utils
{
    internal static class PipeFilters
    {
        public static IList<Element> GetPipesOnView(Document doc, View activeView = null)
        {
            if (activeView == null)
                activeView = doc.ActiveView;

            return new FilteredElementCollector(doc, activeView.Id)
                .OfCategory(BuiltInCategory.OST_PipeCurves)
                .WhereElementIsNotElementType()
                .ToElements();
        }

        public static IList<Element> FilterByLength(IList<Element> pipes, double lengthOption)
        {
            return pipes.Where(tubo =>
            {
                Parameter comprimento = tubo.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
                return comprimento.AsDouble() * 0.3048 > lengthOption;
            }).ToList(); 
        }

        public static IList<Element> RemoveVerticals(IList<Element> pipes)
        {
            return pipes.Where(tubo =>
            {
                LocationCurve pipeCurve = tubo.Location as LocationCurve;
                XYZ startPoint = pipeCurve.Curve.GetEndPoint(0);
                XYZ endPoint = pipeCurve.Curve.GetEndPoint(1);

                return Math.Round(startPoint.X) != Math.Round(endPoint.X) ||
                       Math.Round(startPoint.Y) != Math.Round(endPoint.Y);
            }).ToList();
        }

        public static IList<Element> RemoveViewParallels(IList<Element> pipes)
        {
            return pipes.Where(tubo =>
            {
                LocationCurve pipeCurve = tubo.Location as LocationCurve;
                XYZ startPoint = pipeCurve.Curve.GetEndPoint(0);
                XYZ endPoint = pipeCurve.Curve.GetEndPoint(1);

                return Math.Round(startPoint.X) != Math.Round(endPoint.X) ||
                       Math.Round(startPoint.Z) != Math.Round(endPoint.Z);
            }).ToList();
        }

        public static bool HasPvcMarromPipes(Document doc, IList<Element> pipes)
        {
            return pipes.Any(pipe =>
            {
                ElementId typeId = pipe.GetTypeId();
                Element pipeType = doc.GetElement(typeId);
                return pipeType != null && pipeType.Name == "PVC Marrom Soldável";
            });
        }

        public static IList<Element> GetPvcMarromPipes(Document doc, IList<Element> pipes)
        {
            return pipes.Where(pipe =>
            {
                ElementId typeId = pipe.GetTypeId();
                Element pipeType = doc.GetElement(typeId);
                return pipeType != null && pipeType.Name == "PVC Marrom Soldável";
            }).ToList();
        }

    }

}
