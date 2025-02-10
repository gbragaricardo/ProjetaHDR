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

        internal static IList<string> GetRelativeViewPosition(IList<Element> pipes, ViewDirections viewDirections)
        {
            List<string> relativePositions = new List<string>();

            foreach (Element pipe in pipes)
            {

                LocationCurve pipeCurve = pipe.Location as LocationCurve;
                if (pipeCurve == null) continue;

                // Obtém os pontos extremos da curva do tubo
                XYZ startPoint = pipeCurve.Curve.GetEndPoint(0);
                XYZ endPoint = pipeCurve.Curve.GetEndPoint(1);

                // Calcula a direção do tubo
                XYZ pipeDirection = endPoint - startPoint;

                // Projeta a direção do tubo nos vetores da vista
                double projRight = pipeDirection.DotProduct(viewDirections.Right);
                double projUp = pipeDirection.DotProduct(viewDirections.Up);
                double absProjRight = Math.Abs(pipeDirection.DotProduct(viewDirections.Right));
                double absProjUp = Math.Abs(pipeDirection.DotProduct(viewDirections.Up));

                // Define um limiar para considerar um tubo "estritamente" horizontal ou vertical
                double margin = 0.001;

                // Se a projeção é muito maior em um eixo do que no outro, ele é Horizontal ou Vertical
                if (absProjRight > absProjUp + margin)
                    relativePositions.Add("Horizontal");

                else if (absProjUp > absProjRight + margin)
                    relativePositions.Add("Vertical");

                else if (projUp * projRight > 0)
                    relativePositions.Add("Diagonal Positiva");

                else
                    relativePositions.Add("Diagonal Negativa");
            }

            return relativePositions;
        }

        public static IList<XYZ> GetTaginsertPoint(IList<Element> pipes, string tagMode, ViewDirections viewDirections, IList<string> planPositions = null)
        {
            List<XYZ> insertPoints = new List<XYZ>();
            int clock = 0;

            foreach(Element pipe in pipes)
            {
                LocationCurve curve = pipe.Location as LocationCurve;
                if (curve == null) continue;

                XYZ pontoMedio = curve.Curve.Evaluate(0.5, true);

                if (tagMode == "Flow") 
                { insertPoints.Add(pontoMedio); continue; }

                Parameter diameterParameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER);
                double offset = diameterParameter.AsDouble() / 2;

                XYZ xyzOffset = PipeUtils.AnalyzeOffset(offset, planPositions[clock], tagMode, viewDirections);

                insertPoints.Add(pontoMedio + xyzOffset);

                clock++;
            }

            return insertPoints;
        }

        public static IList<string> GetPipeFlow(IList<Element> elementPipes, bool IsHydraulic, ViewDirections viewDirections)
        {
            IList<string> directions = new List<string>();
            string tempDirection = "Direita";

            foreach (var elementPipe in elementPipes)
            {
                if (elementPipe is Pipe pipe)
                {
                    // Obtém a direção do fluxo para cada tubo
                    string flowDirection = PipeUtils.AnalyzePipeFlow(pipe, tempDirection, IsHydraulic, viewDirections);
                    tempDirection = flowDirection;

                    directions.Add(flowDirection);
                }
            }

            return directions;
        }
    }
}
