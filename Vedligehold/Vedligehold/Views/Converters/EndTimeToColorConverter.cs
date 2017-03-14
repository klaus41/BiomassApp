using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Vedligehold.Views.Converters
{
    class EndTimeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime datetime = new DateTime(1950, 1, 1);
            if (value is DateTime && (DateTime)value > datetime)
                return Color.FromRgb(135, 206, 250);
            //return "green.png";
            else
                return Color.FromRgb(205, 201, 201);
            //return "red.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}