using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Packlists.Model;

namespace Packlists.Converters
{
    public class PacklistsToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var packlists = (ICollection<Packliste>)value;
            if (packlists == null || packlists.Count == 0)
            {
                return "Transparent";
            }
            else
            {
                return "LightGreen";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}