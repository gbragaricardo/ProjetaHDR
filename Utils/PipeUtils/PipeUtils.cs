using Autodesk.Revit.DB;
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
    }
}
