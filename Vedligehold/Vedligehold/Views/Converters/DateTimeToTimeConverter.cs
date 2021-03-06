﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Vedligehold.Views.Converters
{
    class DateTimeToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dt = new DateTime();
            string time = null;
            if (value is DateTime)
            {
                dt = (DateTime)value;
                time = dt.AddHours(2).ToString("HH:mm");
            }
            return time;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
