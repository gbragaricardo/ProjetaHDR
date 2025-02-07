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
    internal static class PipeUtils
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

        public static IList<string> GetPositionOnPlan(IList<Element> pipes)
        {
            List<string> planPositions = new List<string>();
            double margin = 0.01;

            foreach (Element pipe in pipes)
            {
                LocationCurve pipeCurve = pipe.Location as LocationCurve;
                if (pipeCurve == null) continue;

                XYZ startPoint = pipeCurve.Curve.GetEndPoint(0);
                XYZ endPoint = pipeCurve.Curve.GetEndPoint(1);

                XYZ vectors = (endPoint - startPoint).Normalize();

                if (Math.Abs(Math.Abs(vectors.X) - Math.Abs(vectors.Y)) < margin)
                {
                    planPositions.Add((vectors.X * vectors.Y) > 0
                        ? "Diagonal Positiva"
                        : "Diagonal Negativa");
                }
                else if (Math.Abs(vectors.X) > Math.Abs(vectors.Y))
                {
                    planPositions.Add("Horizontal");
                }
                else
                {
                    planPositions.Add("Vertical");
                }
            }

            return planPositions;
        }

        /// <summary>
        /// Calcula o ponto de inserção da tag com base na orientação do tubo.
        /// </summary>
        public static IList<XYZ> GetTaginsertPoint(IList<Element> pipes, IList<string> planPositions, string tagMode)
        {
            List<XYZ> pontosInsercao = new List<XYZ>();

            for (int i = 0; i < pipes.Count; i++)
            {
                Element pipe = pipes[i];
                LocationCurve curve = pipe.Location as LocationCurve;
                if (curve == null) continue;

                XYZ pontoMedio = curve.Curve.Evaluate(0.5, true);

                Parameter parametroDiametro = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER);
                double deslocamento = parametroDiametro.AsDouble() / 2;

                XYZ deslocamentoXYZ;

                switch (planPositions[i])
                {
                    case "Horizontal":
                        deslocamentoXYZ = tagMode == "Diametro"
                            ? new XYZ(0, deslocamento, 0)
                            : new XYZ(0, -deslocamento, 0);
                        break;

                    case "Vertical":
                        deslocamentoXYZ = tagMode == "Diametro"
                            ? new XYZ(-deslocamento, 0, 0)
                            : new XYZ(deslocamento, 0, 0);
                        break;

                    case "Diagonal Positiva":
                        deslocamentoXYZ = tagMode == "Diametro"
                            ? new XYZ(-deslocamento, deslocamento, 0)
                            : new XYZ(deslocamento, -deslocamento, 0);
                        break;

                    case "Diagonal Negativa":
                        deslocamentoXYZ = tagMode == "Diametro"
                            ? new XYZ(deslocamento, deslocamento, 0)
                            : new XYZ(-deslocamento, -deslocamento, 0);
                        break;

                    default:
                        deslocamentoXYZ = XYZ.Zero;
                        break;
                }

                pontosInsercao.Add(pontoMedio + deslocamentoXYZ);
            }

            return pontosInsercao;
        }






    }
}
