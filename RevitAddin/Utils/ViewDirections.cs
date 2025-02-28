using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ProjetaHDR.Utils
{ 
    internal class ViewDirections
    {
        public XYZ Right { get; set; }
        public XYZ Left { get; set; }
        public XYZ Up { get; set; }
        public XYZ Down { get; set; }

        internal ViewDirections(View view)
        {
            if (view is View3D view3D)
            {
                // Em uma vista 3D, Right e Up podem estar desalinhados, então corrigimos usando CrossProduct
                XYZ viewDirection = view3D.ViewDirection; // Direção do olhar da vista 3D

                Right = view3D.RightDirection.Normalize(); // Direção direita corrigida
                Left = Right * -1;
                Up = viewDirection.CrossProduct(Right).Normalize(); // Up perpendicular ao Right
                Down = Up * -1;
            }

            Right = view.RightDirection.Normalize();
            Left = Right * -1;
            Up = view.UpDirection.Normalize();
            Down = Up * -1;
        }
    }
}
