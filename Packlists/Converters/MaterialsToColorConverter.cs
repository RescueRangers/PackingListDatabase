using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Packlists.Model;

namespace Packlists.Converters
{
    public class MaterialsToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var materials = (IList<MaterialAmount>)value;

            if (materials == null || materials.Count == 0)
            {
                return "Red";
            }

            foreach (var item in materials)
            {
                if (item.Material == null) return "Red";
            }

            return "Transparent";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}