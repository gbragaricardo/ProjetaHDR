using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Autodesk.Revit.DB.Plumbing;

namespace ProjetaHDR.UI.Converters
{
    public class OutputPipesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is ObservableCollection<Pipe> outputPipes)
                {
                    var classifiedPipes = outputPipes
                        .GroupBy(pipe => pipe.Name)
                        .Select(group =>
                        {
                            string name = group.Key.Replace("PVC", "").Trim();
                            string diameter = $"{group.First().Diameter * 304.8}";
                            return $"{group.Count()}x {name} - {diameter}mm";
                        })
                        .ToList();

                    return classifiedPipes;
                }
                else
                {
                    return value;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}

