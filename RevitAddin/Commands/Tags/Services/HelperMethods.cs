using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using ProjetaHDR.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR
{
    internal static class HelperMethods
    {
        internal static XYZ AnalyzeOffset(double offset, string planPosition, string tagMode, ViewDirections viewDirections)
        {
            XYZ xyzOffset;
            var up = viewDirections.Up;
            var right = viewDirections.Right;
            var left = viewDirections.Left;
            var down = viewDirections.Down;

            switch (planPosition)
            {
                case "Horizontal":
                    xyzOffset = tagMode == "Diametro"
                        ? up * offset
                        : down * offset;
                    break;

                case "Vertical":
                    xyzOffset = tagMode == "Diametro"
                        ? left * offset
                        : right * offset;
                    break;

                case "Diagonal Positiva":
                    xyzOffset = tagMode == "Diametro"
                        ? (up + left) * (offset * 0.75)
                        : (down + right) * (offset * 0.75);
                    break;

                case "Diagonal Negativa":
                    xyzOffset = tagMode == "Diametro"
                        ? (up + right) * (offset * 0.75)
                        : (down + left) * (offset * 0.75);
                    break;

                default:
                    xyzOffset = XYZ.Zero;
                    break;
            }

            return xyzOffset;
        }

        internal static string AnalyzePipeFlow(Pipe pipe, string tempDirection, bool IsHydraulic, ViewDirections viewDirections)
        {

            ConnectorManager connectorManager = pipe.ConnectorManager;
            if (connectorManager == null)
                return tempDirection;

            // Obtém conectores de entrada e saída
            Connector pipeIn = null, pipeOut = null;

            foreach (Connector connector in connectorManager.Connectors)
            {
                switch (connector.Direction)
                {
                    case FlowDirectionType.In:
                        pipeIn = connector;
                        break;
                    case FlowDirectionType.Out:
                        pipeOut = connector;
                        break;
                }
            }

            // Se não houver entrada ou saída, retorna "Indefinido"
            if (pipeIn == null && pipeOut == null)
                return tempDirection;

            // Calcula a direção do fluxo
            XYZ flowVector = pipeOut.Origin - pipeIn.Origin;

            double projRight = flowVector.DotProduct(viewDirections.Right);
            double projUp = flowVector.DotProduct(viewDirections.Up);

            if (IsHydraulic == true)
            {
                if (Math.Round(projRight, 3) != 0)
                {
                    if (Math.Round(projRight, 3) > 0) return "Direita";
                    if (Math.Round(projRight, 3) < 0) return "Esquerda";
                }
                else
                {
                    if (Math.Round(projUp, 3) > 0) return "Direita";
                    if (Math.Round(projUp, 3) < 0) return "Esquerda";
                }
            }
            else
            {
                if (Math.Round(flowVector.X, 3) != 0)
                {
                    if (Math.Round(flowVector.X, 3) > 0) return "Direita";
                    if (Math.Round(flowVector.X, 3) < 0) return "Esquerda";
                }
                else
                {
                    if (Math.Round(flowVector.Y, 3) > 0) return "Direita";
                    if (Math.Round(flowVector.Y, 3) < 0) return "Esquerda";
                }
            }
            return tempDirection;
        }

    }
}

