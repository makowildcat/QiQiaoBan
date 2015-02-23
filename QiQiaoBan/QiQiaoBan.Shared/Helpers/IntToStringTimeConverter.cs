using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace QiQiaoBan.Helpers
{
    class IntToStringTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int n = (int)value;
            return (n / 60).ToString("00") + ":" + (n % 60).ToString("00");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
