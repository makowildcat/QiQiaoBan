using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace QiQiaoBan.Helpers
{
    /// <summary>
    /// In order to bind directly int to string target
    /// </summary>
    class IntToStringTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return HelpConvert.intToStringTime((int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
