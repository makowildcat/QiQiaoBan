using QiQiaoBan.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Navigation;

namespace QiQiaoBan.ViewModel
{
    public interface IViewModel
    {
        void LoadState(LoadStateEventArgs e);
        void SaveState(SaveStateEventArgs e);
    }
}
