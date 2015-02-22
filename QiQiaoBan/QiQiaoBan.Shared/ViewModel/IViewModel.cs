using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Navigation;

namespace QiQiaoBan.ViewModel
{
    public interface IViewModel
    {
        void NavigateTo(NavigationEventArgs e);
        void NavigateFrom(NavigationEventArgs e);
    }
}
