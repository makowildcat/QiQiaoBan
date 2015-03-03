using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace QiQiaoBan.Helpers
{
    /// <summary>
    /// Little trick to bind Style with Resource
    /// </summary>
    class StringToStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {            
            return App.Current.Resources[value.ToString()];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
