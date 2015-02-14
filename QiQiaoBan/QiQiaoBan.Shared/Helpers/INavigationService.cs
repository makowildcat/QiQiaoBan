using System;
using System.Collections.Generic;
using System.Text;

namespace QiQiaoBan.Helpers
{
    public interface INavigationService
    {
        void Navigate(Type sourcePageType);
        void Navigate(Type sourcePageType, object parameter);
        void GoBack();
    }
}
