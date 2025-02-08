using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProjetaHDR.Utils
{
    internal static class PipeMethods
    {
        
        public static void SetPipeSlope(IList<Element> pipesList)
        {
            if (pipesList == null)
                return;

            foreach (Element pipe in pipesList)
            {
                Parameter slopeParameter = pipe.LookupParameter("PRJ HDR: Inclinacao Tag");

                Parameter parameterAbreviaturaSistema = pipe.get_Parameter(BuiltInParameter.RBS_DUCT_PIPE_SYSTEM_ABBREVIATION_PARAM);
                string abreviaturaSistema = parameterAbreviaturaSistema.AsString();

                Parameter parameterClassificacaoSistema = pipe.get_Parameter(BuiltInParameter.RBS_SYSTEM_CLASSIFICATION_PARAM);
                string classificacaoSistema = parameterClassificacaoSistema.AsString();

                Parameter diameterParameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
                double diameterInMillimeters = diameterParameter.AsDouble() * 304.8;

                if (abreviaturaSistema == "ESG")
                {
                    string slope = (classificacaoSistema == "Sanitário") ? (diameterInMillimeters <= 75 ? "2%" : "1%") : "1%";

                    slopeParameter.Set(slope);
                }
                else if (abreviaturaSistema == "PLUV")
                {
                    slopeParameter.Set("0.5%");
                }
            }
        }

        public static IList<string> GetRelativeViewPosition(IList<Element> pipes, double margin, bool IsHydraulic)
        {
            List<string> relativePositions = new List<string>();

            foreach (Element pipe in pipes)
            {
                LocationCurve pipeCurve = pipe.Location as LocationCurve;
                if (pipeCurve == null) continue;

                XYZ startPoint = pipeCurve.Curve.GetEndPoint(0);
                XYZ endPoint = pipeCurve.Curve.GetEndPoint(1);

                XYZ vectors = (endPoint - startPoint).Normalize();

                relativePositions.Add(PipeUtils.AnalyzePostion(vectors, margin, IsHydraulic));

            }
            return relativePositions;
        }

        public static IList<XYZ> GetTaginsertPoint(IList<Element> pipes, IList<string> planPositions, string tagMode, bool IsHydraulic)
        {
            List<XYZ> insertPoints = new List<XYZ>();
            int clock = 0;

            foreach(Element pipe in pipes)
            {
                LocationCurve curve = pipe.Location as LocationCurve;
                if (curve == null) continue;

                XYZ pontoMedio = curve.Curve.Evaluate(0.5, true);

                Parameter diameterParameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER);
                double offset = diameterParameter.AsDouble() / 2;

                XYZ xyzOffset = PipeUtils.AnalyzeOffset(offset, planPositions[clock], tagMode, IsHydraulic);

                insertPoints.Add(pontoMedio + xyzOffset);

                clock++;
            }

            return insertPoints;
        }






    }
}
