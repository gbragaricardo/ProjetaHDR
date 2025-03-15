using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ProjetaHDR.UI.Converters
{
    public class NullToValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                return null;

            var instanceElementId = values[0] as ElementId;
            if (instanceElementId == null) return null;

            if (values[1] is Dictionary<ElementId, string> dictionary && dictionary.ContainsKey(instanceElementId))
            {
                return instanceElementId;
            }

            return ElementId.InvalidElementId;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[]
            {
                value ?? ElementId.InvalidElementId, // Se o valor for null, retorna InvalidElementId
                System.Windows.Data.Binding.DoNothing // Mantém o dicionário inalterado
            };
        }
    }
}

