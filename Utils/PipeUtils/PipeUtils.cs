using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.Utils
{
    internal static class PipeUtils
    {

        internal static string AnalyzePostion(XYZ vectors, double margin, bool IsHydraulic)
        {
            var absVectorX = Math.Abs(vectors.X);
            var absVectorY = Math.Abs(vectors.Y);
            var absVectorZ = Math.Abs(vectors.Z);

            if (IsHydraulic == true)
            {
                if (Math.Abs(absVectorX - absVectorZ) < margin)
                {
                    return (vectors.X * vectors.Z) > 0
                        ? "Diagonal Negativa"
                        : "Diagonal Positiva";
                }
                else if (absVectorX > absVectorZ)
                {
                    return ("Horizontal");
                }
                else
                {
                    return ("Vertical");
                }
            }
            else 
            {
                if (Math.Abs(absVectorX - absVectorY) < margin)
                    {
                        return (vectors.X * vectors.Y) > 0
                            ? "Diagonal Positiva"
                            : "Diagonal Negativa";
                    }
                    else if (absVectorX > absVectorY)
                    {
                        return("Horizontal");
                    }
                    else
                    {
                        return ("Vertical");
                    }
            }
        }

        internal static XYZ AnalyzeOffset(double offset, string planPosition, string tagMode, bool IsHydraulic)
        {
            XYZ xyzOffset;

            if (IsHydraulic == true)
            {
                switch (planPosition)
                {
                    case "Horizontal":
                        xyzOffset = tagMode == "Diametro"
                            ? new XYZ(0, 0, offset)
                            : new XYZ(0, 0, -offset);
                        break;

                    case "Vertical":
                        xyzOffset = tagMode == "Diametro"
                            ? new XYZ(-offset, 0, 0)
                            : new XYZ(offset, 0, 0);
                        break;

                    case "Diagonal Positiva":
                        xyzOffset = tagMode == "Diametro"
                            ? new XYZ(offset, 0, offset)
                            : new XYZ(-offset, 0, -offset);
                        break;

                    case "Diagonal Negativa":
                        xyzOffset = tagMode == "Diametro"
                            ? new XYZ(-offset, 0, offset)
                            : new XYZ(offset, 0, -offset);
                        break;

                    default:
                        xyzOffset = XYZ.Zero;
                        break;
                }
            }
            else
            {
                switch (planPosition)
                {
                    case "Horizontal":
                        xyzOffset = tagMode == "Diametro"
                            ? new XYZ(0, offset, 0)
                            : new XYZ(0, -offset, 0);
                        break;

                    case "Vertical":
                        xyzOffset = tagMode == "Diametro"
                            ? new XYZ(-offset, 0, 0)
                            : new XYZ(offset, 0, 0);
                        break;

                    case "Diagonal Positiva":
                        xyzOffset = tagMode == "Diametro"
                            ? new XYZ(-offset, offset, 0)
                            : new XYZ(offset, -offset, 0);
                        break;

                    case "Diagonal Negativa":
                        xyzOffset = tagMode == "Diametro"
                            ? new XYZ(offset, offset, 0)
                            : new XYZ(-offset, -offset, 0);
                        break;

                    default:
                        xyzOffset = XYZ.Zero;
                        break;
                }
            }

            return xyzOffset;
        }

        internal static string AnalyzePipeFlow(Pipe pipe, string tempDirection, bool IsHydraulic)
        {
            if (tempDirection == null)
                tempDirection = "Direita";

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

            // Determina a direção com base nos eixos X e Y/Z
            if (Math.Round(flowVector.X, 3) != 0)
            {
                if (Math.Round(flowVector.X, 3) > 0) return "Direita";
                if (Math.Round(flowVector.X, 3) < 0) return "Esquerda";
            }

            if (IsHydraulic == true)
            {
                if (Math.Round(flowVector.Y, 3) > 0) return "Direita";
                if (Math.Round(flowVector.Y, 3) < 0) return "Esquerda";
            }
            else
            {
                if (Math.Round(flowVector.Z, 3) > 0) return "Direita";
                if (Math.Round(flowVector.Z, 3) < 0) return "Esquerda";
            }
            

            return tempDirection;
        }
    }
}
